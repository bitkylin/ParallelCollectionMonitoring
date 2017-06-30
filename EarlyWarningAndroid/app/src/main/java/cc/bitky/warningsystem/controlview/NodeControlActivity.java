package cc.bitky.warningsystem.controlview;

import android.content.Context;
import android.os.Bundle;
import android.support.v4.app.FragmentTabHost;
import android.support.v7.app.AppCompatActivity;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TabHost;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudControl;
import cc.bitky.warningsystem.utils.KyTab;
import cc.bitky.warningsystem.utils.KyToolBar;
import cc.bitky.warningsystem.utils.ToastUtil;
import cn.bmob.v3.exception.BmobException;
import cn.bmob.v3.listener.UpdateListener;

public class NodeControlActivity extends AppCompatActivity {

  private ToastUtil toastUtil;
  Context context;
  List<KyTab> kyTabs = new ArrayList<>(2);
  public FragmentTabHost fragmentTabHost;
  String areaName;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.activity_node_control);
    context = this;
    initTab();
    initView();
    toastUtil = new ToastUtil(this);
    areaName = getIntent().getStringExtra("areaName");

  }

  private void initTab() {
    KyTab tab_control = new KyTab(ControlFragment.class, "控制", R.drawable.navigationbar_selector_control);
    KyTab tab_option = new KyTab(OptionFragment.class, "配置", R.drawable.navigationbar_selector_option);
    kyTabs.add(tab_control);
    kyTabs.add(tab_option);

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

  private void initView() {
    KyToolBar nodeControlActivity_toolbar = (KyToolBar) findViewById(R.id.nodeControlActivity_toolbar);
    nodeControlActivity_toolbar.setRightButtonOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        CloudControl cloudControl = new CloudControl();
        cloudControl.setObjectId("W2obDDDL");
        cloudControl.setDeployCollectOpened(false);
        cloudControl.setDeployProcessOpened(false);
        cloudControl.setCloudCollectProgress(0);
        cloudControl.setCloudProcessOpened(false);
        cloudControl.setCloudCollectOpened(false);
        cloudControl.setCloudProcessProgress(0);
        cloudControl.update(new UpdateListener() {
          @Override
          public void done(BmobException e) {
            if (e == null) {
              toastUtil.show("云服务初始化成功");
            } else {
              toastUtil.show("云服务初始化失败");
            }
          }
        });
      }
    });
    nodeControlActivity_toolbar.setNavigationOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        finish();
      }
    });
  }
}
