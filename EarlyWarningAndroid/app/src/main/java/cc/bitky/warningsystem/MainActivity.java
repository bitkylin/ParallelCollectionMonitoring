package cc.bitky.warningsystem;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.FragmentTabHost;
import android.support.v7.app.AppCompatActivity;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TabHost;
import android.widget.TextView;

import com.baidu.location.BDLocation;
import com.baidu.location.BDLocationListener;
import com.baidu.location.LocationClient;
import com.baidu.location.LocationClientOption;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.fragment.MapFragment;
import cc.bitky.warningsystem.fragment.RecyclerFragment;
import cc.bitky.warningsystem.trial.TrialActivity;
import cc.bitky.warningsystem.utils.KyTab;
import cc.bitky.warningsystem.utils.KyToolBar;
import cc.bitky.warningsystem.utils.LocationListener;

public class MainActivity extends AppCompatActivity {

  Context context;
  List<KyTab> kyTabs = new ArrayList<>(2);
  public FragmentTabHost fragmentTabHost;

  public LocationClient mLocationClient = null;
  private LocationListener locationListener;


  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.activity_main);
    context = this;
    KyToolBar kyToolBar = (KyToolBar) findViewById(R.id.mainActivity_kyToolbar);
    kyToolBar.setRightButtonOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        Intent intent = new Intent(context, TrialActivity.class);
        startActivity(intent);
      }
    });
    initTab();

    //-----------定  位-----------
    mLocationClient = new LocationClient(getApplicationContext());
    //声明LocationClient类
    mLocationClient.registerLocationListener(new MyLocationListener());
    //注册监听函数
    LocationClientOption option = new LocationClientOption();
    option.setLocationMode(LocationClientOption.LocationMode.Hight_Accuracy);//可选，默认高精度，设置定位模式，高精度，低功耗，仅设备
    int span = 5000;
    option.setScanSpan(span);//可选，默认0，即仅定位一次，设置发起定位请求的间隔需要大于等于1000ms才是有效的
    option.setIsNeedAddress(true);//可选，设置是否需要地址信息，默认不需要
    option.setOpenGps(true);//可选，默认false,设置是否使用gps
    option.setLocationNotify(true);//可选，默认false，设置是否当gps有效时按照1S1次频率输出GPS结果
    option.setIsNeedLocationDescribe(true);//可选，默认false，设置是否需要位置语义化结果，可以在BDLocation.getLocationDescribe里得到，结果类似于“在北京天安门附近”
    option.setIsNeedLocationPoiList(true);//可选，默认false，设置是否需要POI结果，可以在BDLocation.getPoiList里得到
    option.setCoorType("bd09ll");
    mLocationClient.setLocOption(option);
    mLocationClient.start();
  }


  private void initTab() {
    KyTab tab_home = new KyTab(MapFragment.class, "地图", R.drawable.navigationbar_selector_category);
    KyTab tab_hot = new KyTab(RecyclerFragment.class, "分类", R.drawable.navigationbar_selector_hot);
    kyTabs.add(tab_home);
    kyTabs.add(tab_hot);

    fragmentTabHost = (FragmentTabHost) findViewById(R.id.fragmentTabHost);
    fragmentTabHost.setup(context, getSupportFragmentManager(), R.id.fragment);

    for (KyTab tab : kyTabs) {
      TabHost.TabSpec tabSpec =
          fragmentTabHost.newTabSpec(tab.getTitle()).setIndicator(buildIndicator(tab));
      fragmentTabHost.addTab(tabSpec, tab.getFragment(), null);
    }

    fragmentTabHost.getTabWidget().setShowDividers(LinearLayout.SHOW_DIVIDER_NONE);
    fragmentTabHost.setCurrentTab(0);
  }

  private View buildIndicator(KyTab tab) {
    View view = LayoutInflater.from(context).inflate(R.layout.tab_indicator, null);
    ImageView img = (ImageView) view.findViewById(R.id.icon_tab);
    TextView text = (TextView) view.findViewById(R.id.txt_indicator);
    img.setBackgroundResource(tab.getIcon());
    text.setText(tab.getTitle());
    return view;
  }

  public void setLocationListener(LocationListener locationListener) {
    this.locationListener = locationListener;
  }

  public class MyLocationListener implements BDLocationListener {
    @Override
    public void onReceiveLocation(BDLocation bdLocation) {
      if (locationListener != null) locationListener.locate(bdLocation);
    }

    @Override
    public void onConnectHotSpotMessage(String s, int i) {

    }
  }
}
