package cc.bitky.warningsystem.bean;

public class AreaItem {
  private String name;
  private String time;

  public AreaItem(String name) {
    this.name = name;
  }

  public AreaItem(String name, String time) {
    this.name = name;
    this.time = time;
  }

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public String getTime() {
    return time;
  }

  public void setTime(String time) {
    this.time = time;
  }
}
