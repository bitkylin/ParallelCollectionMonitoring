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

import com.baidu.mapapi.map.BaiduMap;
import com.baidu.mapapi.map.BitmapDescriptorFactory;
import com.baidu.mapapi.map.InfoWindow;
import com.baidu.mapapi.map.MapStatusUpdate;
import com.baidu.mapapi.map.MapStatusUpdateFactory;
import com.baidu.mapapi.map.Marker;
import com.baidu.mapapi.map.MarkerOptions;
import com.baidu.mapapi.map.MyLocationConfiguration;
import com.baidu.mapapi.map.MyLocationData;
import com.baidu.mapapi.map.OverlayOptions;
import com.baidu.mapapi.map.TextureMapView;
import com.baidu.mapapi.model.LatLng;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.MainActivity;
import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudDevice;
import cc.bitky.warningsystem.category.CategoryActivity;
import cc.bitky.warningsystem.controlview.NodeControlActivity;
import cn.bmob.v3.BmobQuery;
import cn.bmob.v3.datatype.BmobGeoPoint;
import cn.bmob.v3.exception.BmobException;
import cn.bmob.v3.listener.FindListener;


public class MapFragment extends Fragment implements CompoundButton.OnCheckedChangeListener, BaiduMap.OnMarkerClickListener, View.OnClickListener {

    private View view;
    private TextureMapView mMapView;
    private BaiduMap baiduMap;
    private boolean isFirstLocate = false;
    MyLocationData locData;
    View overlay;
    MarkerOptions option = new MarkerOptions();

    TextView tvAddOverlayItemUserID;
    TextView tvAddOverlayItemLatlng;
    boolean isPause = false;
    private TextView tvAddOverlayStatus;
    private Context context;

    Thread threadBmob;

    public MapFragment() {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        context = getContext();
        MainActivity activity = (MainActivity) getActivity();
        activity.setLocationListener(location -> {
            locData = new MyLocationData.Builder()
                    .accuracy(location.getRadius())
                    .latitude(location.getLatitude())
                    .longitude(location.getLongitude()).build();
            setBaiduMapLocationData(locData);
            Log.d("lml", "locate: " + location.getLatitude() + "," + location.getLongitude());
        });
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Log.d("lml", "onCreateView");
        view = inflater.inflate(R.layout.fragment_map, container, false);
        //管理员权限用户信息弹窗初始化
        overlay = View.inflate(getContext(), R.layout.item_map_addoverlay_device, null);
        tvAddOverlayItemUserID = (TextView) overlay.findViewById(R.id.tvAddOverlayItemUserID);
        tvAddOverlayStatus = (TextView) overlay.findViewById(R.id.tvAddOverlayStatus);
        tvAddOverlayItemLatlng = (TextView) overlay.findViewById(R.id.tvAddOverlayItemLatlng);
        Button btnAddOverlayItem_msg = (Button) overlay.findViewById(R.id.btnAddOverlayItem_msg);
        Button btnAddOverlayItem_operate = (Button) overlay.findViewById(R.id.btnAddOverlayItem_operate);
        btnAddOverlayItem_msg.setOnClickListener(this);
        btnAddOverlayItem_operate.setOnClickListener(this);
        Switch mapfragmentSwitchlocation = view.findViewById(R.id.mapFragment_switchLocation);
        mapfragmentSwitchlocation.setOnCheckedChangeListener(this);
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
        refreshMapUI(new ArrayList<>(0));

        if (threadBmob == null) {
            threadBmob = new Thread(() -> {
                while (true) {
                    if (!isPause) {
                        BmobQuery<CloudDevice> bmobQuery = new BmobQuery<>();
                        bmobQuery.addWhereEqualTo("enabled", true);
                        bmobQuery.findObjects(new FindListener<CloudDevice>() {
                            @Override
                            public void done(List<CloudDevice> object, BmobException e) {
                                if (e == null) {
                                    if (isPause) {
                                        return;
                                    }
                                    refreshMapUI(object);
                                } else {
                                    Log.i("bmob", "失败：" + e.getMessage() + "," + e.getErrorCode());
                                }
                            }
                        });
                    }
                    try {
                        Thread.sleep(5000);
                        if (threadBmob == null) {
                            return;
                        }
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
            });
            threadBmob.start();
        }
        return view;
    }

    /**
     * 刷新地图中的覆盖物
     *
     * @param devices 从 bmob 获取的设备集合
     */
    private void refreshMapUI(List<CloudDevice> devices) {
        List<OverlayOptions> options = new ArrayList<>();
        for (CloudDevice device : devices) {
            Bundle bundle = new Bundle();
            bundle.putString("name", device.getName());
            bundle.putInt("status", device.getStatus());
            bundle.putBoolean("ex", device.getException());
            option.extraInfo(bundle);
            option.icon(BitmapDescriptorFactory.fromResource(device.getException() ? R.mipmap.map_mark_device_red : R.mipmap.map_mark_device_blue));
            BmobGeoPoint geoPoint = device.getGeo();
            option.position(new LatLng(geoPoint.getLatitude(), geoPoint.getLongitude()));
            options.add(option);
            Log.d("lml", "refreshMapUI: Overlay已刷新");
        }
        baiduMap.clear();
        baiduMap.addOverlays(options);
    }

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
        isPause = false;
        //在activity执行onResume时执行mMapView. onResume ()，实现地图生命周期管理
        mMapView.onResume();
    }

    @Override
    public void onPause() {
        super.onPause();
        isPause = true;
        baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.NORMAL, false, null));
        //在activity执行onPause时执行mMapView. onPause ()，实现地图生命周期管理
        mMapView.onPause();
    }

    @Override
    public void onStop() {
        super.onStop();
        threadBmob = null;
    }

    @Override
    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        switch (buttonView.getId()) {
            case R.id.mapFragment_switchLocation:
                if (isChecked) {
                    baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.FOLLOWING, false, null));
                    baiduMap.animateMapStatus(MapStatusUpdateFactory.zoomTo(baiduMap.getMaxZoomLevel() - 4));
                } else {
                    baiduMap.setMyLocationConfigeration(new MyLocationConfiguration(MyLocationConfiguration.LocationMode.NORMAL, false, null));
                }
                break;
            default:
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
            int statusValue = bundle.getInt("status");
            if (statusValue >= 100 && statusValue < 200) {
                tvAddOverlayStatus.setText("采集中 " + (statusValue - 100) + "%");
            } else if (statusValue >= 200 && statusValue < 300) {
                tvAddOverlayStatus.setText("处理中 " + (statusValue - 200) + "%");
            } else {
                tvAddOverlayStatus.setText("未采集");
            }
            tvAddOverlayItemLatlng.setText("坐标：" + latLngshow.longitude + " , " + latLngshow.latitude);
            baiduMap.showInfoWindow(new InfoWindow(overlay, marker.getPosition(), -90));

            MapStatusUpdate update = MapStatusUpdateFactory.newLatLng(marker.getPosition());
            baiduMap.animateMapStatus(update);
            return true;
        } else {
            return false;
        }
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
            default:
        }
    }
}
