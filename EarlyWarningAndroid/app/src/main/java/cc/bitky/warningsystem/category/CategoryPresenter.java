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
    }

    return categoryNames;
  }
}
