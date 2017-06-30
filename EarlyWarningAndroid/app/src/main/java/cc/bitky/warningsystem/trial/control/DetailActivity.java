package cc.bitky.warningsystem.trial.control;

import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.facebook.drawee.view.SimpleDraweeView;

import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudAreaItem;
import cc.bitky.warningsystem.utils.KyToolBar;
import cn.bmob.v3.BmobQuery;
import cn.bmob.v3.exception.BmobException;
import cn.bmob.v3.listener.QueryListener;

public class DetailActivity extends AppCompatActivity {

  private KyToolBar detailActivity_toolbar;
  private TextView detailActivity_textTitle;
  private TextView detailActivity_coordinate;
  private TextView detailActivity_textStatus;
  private TextView detailActivity_textDetail;
  private SimpleDraweeView detailActivity_draweeview;
  private LinearLayout detailActivity_Layout_prepare;
  private LinearLayout detailActivity_Layout_show;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.activity_detail);
    initView();
    detailActivity_Layout_prepare.setVisibility(View.VISIBLE);
    detailActivity_Layout_show.setVisibility(View.GONE);
    String objectId = getIntent().getStringExtra("objectId");
    updateDetailShow(objectId);
  }

  private void initView() {
    detailActivity_Layout_prepare = (LinearLayout) findViewById(R.id.detailActivity_Layout_prepare);
    detailActivity_Layout_show = (LinearLayout) findViewById(R.id.detailActivity_Layout_show);
    detailActivity_toolbar = (KyToolBar) findViewById(R.id.detailActivity_toolbar);
    detailActivity_textTitle = (TextView) findViewById(R.id.detailActivity_textTitle);
    detailActivity_textStatus = (TextView) findViewById(R.id.detailActivity_textStatus);
    detailActivity_coordinate = (TextView) findViewById(R.id.detailActivity_coordinate);
    detailActivity_textDetail = (TextView) findViewById(R.id.detailActivity_textDetail);
    detailActivity_draweeview = (SimpleDraweeView) findViewById(R.id.detailActivity_draweeview);
    detailActivity_toolbar.setNavigationOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        finish();
      }
    });
  }

  private void updateDetailShow(String objectId) {
    BmobQuery<CloudAreaItem> bmobQuery = new BmobQuery<>();
    bmobQuery.getObject(objectId, new QueryListener<CloudAreaItem>() {
      @Override
      public void done(CloudAreaItem cloudAreaItem, BmobException e) {
        if (e == null && cloudAreaItem != null) {
          updateDetailData(cloudAreaItem);
          //注意：这里的Person对象中只有指定列的数据。
        } else {
          Log.i("bmob", "失败：" + e.getMessage() + "," + e.getErrorCode());
        }
      }
    });
  }

  public void updateDetailData(CloudAreaItem data) {

    detailActivity_Layout_prepare.setVisibility(View.GONE);
    detailActivity_Layout_show.setVisibility(View.VISIBLE);
    if (data.getProcessBar() != null && data.getProcessBar() == 100) {
      detailActivity_textDetail.setText(data.getDetail());
      detailActivity_textTitle.setText("测点「1」");
      if (data.getStatus()) {
        detailActivity_textStatus.setText("正常");
        detailActivity_textStatus.setTextColor(getResources().getColor(R.color.black));

      } else {
        detailActivity_textStatus.setText("异常");
        detailActivity_textStatus.setTextColor(getResources().getColor(R.color.red));
      }
      detailActivity_draweeview.setImageURI(data.getPhotoUri());
      detailActivity_coordinate.setText(data.getCoordinate());
    }
  }
}
