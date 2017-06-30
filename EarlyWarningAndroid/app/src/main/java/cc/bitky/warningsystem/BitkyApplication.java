package cc.bitky.warningsystem;

import android.app.Application;

import com.baidu.mapapi.SDKInitializer;
import com.facebook.drawee.backends.pipeline.Fresco;

import cn.bmob.v3.Bmob;

public class BitkyApplication extends Application {

  final String Bmob_application_ID = "d8600d6f5011f0bc2e9650d4d8e32711";

  @Override
  public void onCreate() {
    super.onCreate();
    SDKInitializer.initialize(this);
    Fresco.initialize(this);
    Bmob.initialize(this, Bmob_application_ID);
  }
}
