package cc.bitky.warningsystem.bean;

import cn.bmob.v3.BmobObject;
import cn.bmob.v3.datatype.BmobGeoPoint;

public class CloudDevice extends BmobObject {

  private String name;
  private BmobGeoPoint geo;
  private Boolean exception;
  private Boolean enabled;
  private Integer status = 0;

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public BmobGeoPoint getGeo() {
    return geo;
  }

  public void setGeo(BmobGeoPoint geo) {
    this.geo = geo;
  }

  public Boolean getException() {
    return exception;
  }

  public void setException(Boolean exception) {
    this.exception = exception;
  }

  public Boolean getEnabled() {
    return enabled;
  }

  public void setEnabled(Boolean enabled) {
    this.enabled = enabled;
  }

  public Integer getStatus() {
    return status;
  }

  public void setStatus(Integer status) {
    this.status = status;
  }
}
