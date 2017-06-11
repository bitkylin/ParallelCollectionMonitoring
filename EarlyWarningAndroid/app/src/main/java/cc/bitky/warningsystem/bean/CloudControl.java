package cc.bitky.warningsystem.bean;

import cn.bmob.v3.BmobObject;


public class CloudControl extends BmobObject {
  Integer cloudProcessProgress;
  Boolean cloudProcessOpened;
  Boolean deployProcessOpened;
  Integer cloudCollectProgress;
  Boolean cloudCollectOpened;
  Boolean deployCollectOpened;


  public Integer getCloudProcessProgress() {
    return cloudProcessProgress;
  }

  public void setCloudProcessProgress(Integer cloudProcessProgress) {
    this.cloudProcessProgress = cloudProcessProgress;
  }

  public Boolean getCloudProcessOpened() {
    return cloudProcessOpened;
  }

  public void setCloudProcessOpened(Boolean cloudProcessOpened) {
    this.cloudProcessOpened = cloudProcessOpened;
  }

  public Integer getCloudCollectProgress() {
    return cloudCollectProgress;
  }

  public void setCloudCollectProgress(Integer cloudCollectProgress) {
    this.cloudCollectProgress = cloudCollectProgress;
  }

  public Boolean getCloudCollectOpened() {
    return cloudCollectOpened;
  }

  public void setCloudCollectOpened(Boolean cloudCollectOpened) {
    this.cloudCollectOpened = cloudCollectOpened;
  }

  public Boolean getDeployProcessOpened() {
    return deployProcessOpened;
  }

  public void setDeployProcessOpened(Boolean deployProcessOpened) {
    this.deployProcessOpened = deployProcessOpened;
  }

  public Boolean getDeployCollectOpened() {
    return deployCollectOpened;
  }

  public void setDeployCollectOpened(Boolean deployCollectOpened) {
    this.deployCollectOpened = deployCollectOpened;
  }
}
