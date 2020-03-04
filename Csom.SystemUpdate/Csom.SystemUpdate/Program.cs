using System;
using System.Configuration;
using System.Threading;
using Csom.Library;
using Microsoft.SharePoint.Client;

namespace Csom.SystemUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            var account = ConfigurationManager.AppSettings["account"];
            var password = ConfigurationManager.AppSettings["password"];
            var webUrl = ConfigurationManager.AppSettings["url"];

            var spService = new SPService(account, password, webUrl);

            using (spService)
            {
                var context = spService.Context;

                var list = context.Web.GetListByUrl("/Lists/SystemUpdate");
                context.Load(list);
                context.ExecuteQueryRetry();

                // アイテム作成情報
                var creationInformation = new ListItemCreationInformation();

                // item1: 何もしない（比較対象）
                var item1 = list.AddItem(creationInformation);
                item1["Title"] = "item1";
                item1.Update();
                context.ExecuteQueryRetry();

                // item2: 作成日を過去日（２日前）に変更
                // ⇒ 結果: 作成日が２日前にならなかった。SystemUpdate なので作成日と更新日が同じまま
                var item2 = list.AddItem(creationInformation);
                item2["Title"] = "item2";
                item2.Update();
                context.ExecuteQueryRetry();

                Thread.Sleep(60 * 1000);

                item2["Title"] = "item2-Update";
                item2["Created"] = DateTime.Now.AddDays(-2);
                item2.SystemUpdate();
                context.ExecuteQueryRetry();

                // item3: 更新日を未来日（２日後）に変更
                // ⇒ 結果: 更新日が２日後にならなかった。SystemUpdate なので作成日と更新日が同じまま
                var item3 = list.AddItem(creationInformation);
                item3["Title"] = "item3";
                item3.Update();
                context.ExecuteQueryRetry();

                Thread.Sleep(60 * 1000);

                item3["Title"] = "item3-Update";
                item3["Modified"] = DateTime.Now.AddDays(2);
                item3.SystemUpdate();
                context.ExecuteQueryRetry();

                // item4: 1分置いてから更新して、さらに1分置いて更新日を作成日に戻す
                // ⇒ 結果：更新時間が作成時間の1分後（SystemUpdate() ではなく Update() だったら2分後になってる）
                var item4 = list.AddItem(creationInformation);
                item4["Title"] = "item3";
                item4.Update();
                context.ExecuteQueryRetry();

                var now = DateTime.Now;

                Thread.Sleep(60 * 1000);

                item4["Title"] = "item3-Update";
                item4.Update();
                context.ExecuteQueryRetry();

                Thread.Sleep(60 * 1000);

                item4["Title"] = "item3-Update-Modified";
                item4["Modified"] = now;
                item4.SystemUpdate();
                context.ExecuteQueryRetry();
            }

            Console.WriteLine("Press any key...");
            Console.ReadLine();
        }
    }
}
