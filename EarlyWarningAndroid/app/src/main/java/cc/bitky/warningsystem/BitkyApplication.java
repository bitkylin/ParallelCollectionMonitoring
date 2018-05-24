package cc.bitky.warningsystem;

import android.app.Application;

import com.baidu.mapapi.SDKInitializer;
import com.facebook.drawee.backends.pipeline.Fresco;

import cn.bmob.v3.Bmob;

public class BitkyApplication extends Application {

  final String Bmob_application_ID = "你的 ID";

  @Override
  public void onCreate() {
    super.onCreate();
    SDKInitializer.initialize(this);
    Fresco.initialize(this);
    Bmob.initialize(this, Bmob_application_ID);
  }
}
