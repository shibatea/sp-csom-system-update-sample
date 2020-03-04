# sp-csom-system-update-sample
CSOM の SystemUpdate メソッドの動作を確認するサンプルです

# 検証結果
![検証結果](https://github.com/shibatea/sp-csom-system-update-sample/blob/master/Result.png)

- item1
  - 比較対象
- item2
  - 作成日を過去日（２日前）に設定して SystemUpdate
    - 結果: 作成日が２日前にならなかった。SystemUpdate なので作成日と更新日が同じまま
- item3
  - 更新日を未来日（２日後）に設定して SystemUpdate
    - 結果: 更新日が２日後にならなかった。SystemUpdate なので作成日と更新日が同じまま
- item4
  - 1分置いてから更新(Update)して、さらに1分置いて更新日を作成日に戻す(SystemUpdate)
    - 結果：更新時間が作成時間の1分後になる（2回目の更新が SystemUpdate ではなく Update だったら2分後になってる）
  
