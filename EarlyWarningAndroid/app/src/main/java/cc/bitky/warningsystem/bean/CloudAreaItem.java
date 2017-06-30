package cc.bitky.warningsystem.bean;

import cn.bmob.v3.BmobObject;
import cn.bmob.v3.datatype.BmobDate;

public class CloudAreaItem extends BmobObject {
  /**
   * 地点名
   */
  private String name;
  /**
   * 状态详情
   */
  private String detail;
  /**
   * 当前监测状态
   */
  private Boolean status;
  private Boolean enabled;
  /**
   * 生成时间
   */
  private BmobDate time;
  /**
   * 图片地址
   */
  private String photoUri;
  /**
   * 反演进度
   */
  private Integer processBar;

  private String coordinate;

  public String getCoordinate() {
    return coordinate;
  }

  public void setCoordinate(String coordinate) {
    this.coordinate = coordinate;
  }


  public CloudAreaItem(String name, BmobDate time) {
    this.name = name;
    this.time = time;
  }

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public String getDetail() {
    return detail;
  }

  public void setDetail(String detail) {
    this.detail = detail;
  }

  public Boolean getStatus() {
    return status;
  }

  public void setStatus(Boolean status) {
    this.status = status;
  }

  public BmobDate getTime() {
    return time;
  }

  public void setTime(BmobDate time) {
    this.time = time;
  }

  public String getPhotoUri() {
    return photoUri;
  }

  public void setPhotoUri(String photoUri) {
    this.photoUri = photoUri;
  }

  public Integer getProcessBar() {
    return processBar;
  }

  public void setProcessBar(Integer processBar) {
    this.processBar = processBar;
  }

  public Boolean getEnabled() {
    return enabled;
  }

  public void setEnabled(Boolean enabled) {
    this.enabled = enabled;
  }
}
