package cc.bitky.warningsystem.category;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.bean.CloudAreaItem;

public class CategoryPresenter {

  private List<CloudAreaItem> categoryNames;
  private CategoryActivity activity;

  public CategoryPresenter(CategoryActivity activity) {

    this.activity = activity;
  }

  public List<CloudAreaItem> getCategoryNames() {
    //TODO
    if (categoryNames == null) {
      categoryNames = new ArrayList<>();
//      categoryNames.add(new CloudAreaItem("正在获取", new BmobDate(new Date())));
//      categoryNames.add(new AreaItem("桂电校园", new Date(2017, 4, 3)));
//      categoryNames.add(new AreaItem("七百弄", new Date()));
//      categoryNames.add(new AreaItem("无人区域", new Date(2017, 12, 31)));
    }

    return categoryNames;
  }
}
