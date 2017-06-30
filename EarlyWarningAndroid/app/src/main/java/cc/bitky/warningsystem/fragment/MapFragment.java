package cc.bitky.warningsystem.fragment;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.TextView;

import com.baidu.location.BDLocation;
import com.baidu.mapapi.map.BaiduMap;
import com.baidu.mapapi.map.BitmapDescriptorFactory;
import com.baidu.mapapi.map.InfoWindow;
import com.baidu.mapapi.map.MapStatusUpdate;
import com.baidu.mapapi.map.MapStatusUpdateFactory;
import com.baidu.mapapi.map.Marker;
import com.baidu.mapapi.map.MarkerOptions;
import com.baidu.mapapi.map.MyLocationConfiguration;
import com.baidu.mapapi.map.MyLocationData;
import com.baidu.mapapi.map.TextureMapView;
import com.baidu.mapapi.model.LatLng;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.MainActivity;
import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudDevice;
import cc.bitky.warningsystem.category.CategoryActivity;
import cc.bitky.warningsystem.controlview.NodeControlActivity;
import cc.bitky.warningsystem.utils.LocationListener;
import cn.bmob.v3.BmobQuery;
import cn.bmob.v3.datatype.BmobGeoPoint;
import cn.bmob.v3.exception.BmobException;
import cn.bmob.v3.listener.FindListener;


public class MapFragment extends Fragment implements CompoundButton.OnCheckedChangeListener, BaiduMap.OnMarkerClickListener, View.OnClickListener {

  private View view;
  private TextureMapView mMapView;
  private Switch mapFragment_switchLocation;
  private BaiduMap baiduMap;
  private boolean isFirstLocate = false;
  MyLocationData locData;
  View overlay;
  MarkerOptions option = new MarkerOptions();

  TextView tvAddOverlayItemUserID;
  TextView tvAddOverlayItemLatlng;
  boolean threadStoped = false;
  Thread thread;
  private TextView tvAddOverlayStatus;
  private Context context;

  public MapFragment() {
    // Required empty public constructor
  }

  @Override
  public void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    context = getContext();
    MainActivity activity = (MainActivity) getActivity();
    activity.setLocationListener(new LocationListener() {
      @Override
      public void locate(BDLocation location) {

        locData = new MyLocationData.Builder()
            .accuracy(location.getRadius())
            .latitude(location.getLatitude())
            .longitude(location.getLongitude()).build();
        setBaiduMapLocationData(locData);
        Log.d("lml", "locate: " + location.getRadius());
      }
    });

    //管理员权限用户信息弹窗初始化
    overlay = View.inflate(getContext(), R.layout.item_map_addoverlay_device, null);
    tvAddOverlayItemUserID = (TextView) overlay.findViewById(R.id.tvAddOverlayItemUserID);
    tvAddOverlayStatus = (TextView) overlay.findViewById(R.id.tvAddOverlayStatus);
    tvAddOverlayItemLatlng = (TextView) overlay.findViewById(R.id.tvAddOverlayItemLatlng);
    Button btnAddOverlayItem_msg = (Button) overlay.findViewById(R.id.btnAddOverlayItem_msg);
    Button btnAddOverlayItem_operate = (Button) overlay.findViewById(R.id.btnAddOverlayItem_operate);
    btnAddOverlayItem_msg.setOnClickListener(this);
    btnAddOverlayItem_operate.setOnClickListener(this);
  }


  @Override
  public View onCreateView(LayoutInflater inflater, ViewGroup container,
                           Bundle savedInstanceState) {
    view = inflater.inflate(R.layout.fragment_map, container, false);

    mapFragment_switchLocation = (Switch) view.findViewById(R.id.mapFragment_switchLocation);
    mapFragment_switchLocation.setOnCheckedChangeListener(this);
    mMapView = (TextureMapView) view.findViewById(R.id.bmapView);
    baiduMap = mMapView.getMap();
    MapStatusUpdate u1 = MapStatusUpdateFactory.newLatLngZoom(new LatLng(25.53906, 110.30269), baiduMap.getMaxZoomLevel() - 6);
    baiduMap.animateMapStatus(u1);//动画移动摄像头
    baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.NORMAL, false, null));
    // 开启定位图层
    baiduMap.setMyLocationEnabled(true);
    baiduMap.setOnMarkerClickListener(this);
    //再次进入地图fragment时界面刷新
    if (locData != null) {
      MapStatusUpdate u = MapStatusUpdateFactory.newLatLngZoom(new LatLng(locData.latitude, locData.longitude), baiduMap.getMaxZoomLevel() - 6);
      baiduMap.animateMapStatus(u);//动画移动摄像头
    }
    refreshMapUI(new ArrayList<CloudDevice>(0));


    thread = new Thread(new Runnable() {
      @Override
      public void run() {
        while (!threadStoped) {
          BmobQuery<CloudDevice> bmobQuery = new BmobQuery<>();
          bmobQuery.addWhereEqualTo("enabled", true);
          bmobQuery.findObjects(new FindListener<CloudDevice>() {
            @Override
            public void done(List<CloudDevice> object, BmobException e) {
              if (threadStoped) return;
              if (e == null) {
                refreshMapUI(object);
              } else {
                Log.i("bmob", "失败：" + e.getMessage() + "," + e.getErrorCode());
              }
            }
          });
          try {
            Thread.sleep(5000);
          } catch (InterruptedException e) {
            e.printStackTrace();
          }
        }
      }
    });


    return view;
  }

  //刷新地图中的覆盖物
  private void refreshMapUI(List<CloudDevice> devices) {
    baiduMap.clear();
    for (CloudDevice device : devices) {
      Bundle bundle = new Bundle();
      bundle.putString("name", device.getName());
      bundle.putInt("status", device.getStatus());
      bundle.putBoolean("ex", device.getException());
      option.extraInfo(bundle);
      option.icon(BitmapDescriptorFactory.fromResource(device.getException() ? R.mipmap.map_mark_device_red : R.mipmap.map_mark_device_blue));
      BmobGeoPoint geoPoint = device.getGeo();
      option.position(new LatLng(geoPoint.getLatitude(), geoPoint.getLongitude()));
      baiduMap.addOverlay(option);
      Log.d("lml", "refreshMapUI: Overlay已刷新");
    }
  }


//  public void refreshMapUI() {
//    baiduMap.clear();
//    Bundle bundle = new Bundle();
//    bundle.putString("name", "节点「1」");
//    bundle.putBoolean("status", true);
//    option.icon(BitmapDescriptorFactory.fromResource(R.mipmap.map_mark_device_blue));
//    option.position(new LatLng(25.288406, 110.343313));
//    baiduMap.addOverlay(option);
//
//    option.icon(BitmapDescriptorFactory.fromResource(R.mipmap.map_mark_device_red));
//    option.position(new LatLng(25.288178, 110.345649));
//    baiduMap.addOverlay(option);
//  }

  @Override
  public void onAttach(Context context) {
    super.onAttach(context);
  }

  @Override
  public void onDetach() {
    super.onDetach();
  }

  @Override
  public void onDestroy() {
    super.onDestroy();
    //在activity执行onDestroy时执行mMapView.onDestroy()，实现地图生命周期管理
    mMapView.onDestroy();
  }

  @Override
  public void onResume() {
    super.onResume();
    threadStoped = false;
    thread.start();
    //在activity执行onResume时执行mMapView. onResume ()，实现地图生命周期管理
    mMapView.onResume();
  }

  @Override
  public void onPause() {
    super.onPause();
    baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.NORMAL, false, null));
    //在activity执行onPause时执行mMapView. onPause ()，实现地图生命周期管理
    mMapView.onPause();
  }

  @Override
  public void onStop() {
    super.onStop();
    threadStoped = true;
  }

  @Override
  public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
    switch (buttonView.getId()) {
      case R.id.mapFragment_switchLocation:
        if (isChecked) {
          baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.FOLLOWING, false, null));
          baiduMap.animateMapStatus(MapStatusUpdateFactory.zoomTo(baiduMap.getMaxZoomLevel() - 4));
        } else
          baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.NORMAL, false, null));
        break;
    }
  }

  public void setBaiduMapLocationData(MyLocationData locData) {
    baiduMap.setMyLocationData(locData);
    if (isFirstLocate) {
      MapStatusUpdate u = MapStatusUpdateFactory.newLatLngZoom(new LatLng(locData.latitude, locData.longitude), baiduMap.getMaxZoomLevel() - 6);
      baiduMap.animateMapStatus(u);//动画移动摄像头
      isFirstLocate = false;
    }
    Log.d("lml", "MapFragment - 设置一次位置");
  }

  @Override
  public boolean onMarkerClick(Marker marker) {
    baiduMap.hideInfoWindow();
    if (marker != null) {
      LatLng latLngshow = marker.getPosition();
      Bundle bundle = marker.getExtraInfo();
      tvAddOverlayItemUserID.setText(bundle.getString("name"));
      overlay.setBackgroundColor(getResources().getColor(bundle.getBoolean("ex") ? R.color.orangered : R.color.deepskyblue));
      switch (bundle.getInt("status")) {
        case 0:
          tvAddOverlayStatus.setText("未采集");
          break;
        case 1:
          tvAddOverlayStatus.setText("采集中");
          break;
      }
      tvAddOverlayItemLatlng.setText("坐标：" + latLngshow.longitude + " , " + latLngshow.latitude);
      baiduMap.showInfoWindow(new InfoWindow(overlay, marker.getPosition(), -90));
      MapStatusUpdate update = MapStatusUpdateFactory.newLatLng(marker.getPosition());
      baiduMap.animateMapStatus(update);
      return true;
    } else
      return false;
  }

  @Override
  public void onClick(View v) {
    Intent intent;
    switch (v.getId()) {
      //查看详情
      case R.id.btnAddOverlayItem_msg:
        baiduMap.hideInfoWindow();
        intent = new Intent(context, CategoryActivity.class);

        intent.putExtra("areaName", tvAddOverlayItemUserID.getText());
        startActivity(intent);
        break;
      //进行操作
      case R.id.btnAddOverlayItem_operate:
        baiduMap.hideInfoWindow();
        intent = new Intent(context, NodeControlActivity.class);
        intent.putExtra("areaName", tvAddOverlayItemUserID.getText());
        startActivity(intent);
        break;
    }
  }
}
