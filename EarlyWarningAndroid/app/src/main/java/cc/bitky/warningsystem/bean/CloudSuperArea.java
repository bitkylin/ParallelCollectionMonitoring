package cc.bitky.warningsystem.bean;

import cn.bmob.v3.BmobObject;

public class CloudSuperArea extends BmobObject {
  private String name;
  private String photoUri;
  private boolean status;

  public CloudSuperArea(String name, String photoUri, boolean status) {
    this.name = name;
    this.photoUri = photoUri;
    this.status = status;
  }


  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public String getPhotoUri() {
    return photoUri;
  }

  public void setPhotoUri(String photoUri) {
    this.photoUri = photoUri;
  }

  public boolean isStatus() {
    return status;
  }

  public void setStatus(boolean status) {
    this.status = status;
  }
}
