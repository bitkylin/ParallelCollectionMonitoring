package cc.bitky.warningsystem.trial;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.DefaultItemAnimator;
import android.support.v7.widget.GridLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.View;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudSuperArea;
import cc.bitky.warningsystem.controlview.NodeControlActivity;
import cc.bitky.warningsystem.utils.KyBaseRecyclerAdapter;
import cc.bitky.warningsystem.utils.KyBaseViewHolder;
import cc.bitky.warningsystem.utils.KyToolBar;

public class TrialActivity extends AppCompatActivity {

  private KyToolBar trialActivity_toolbar;
  Context context;
  private RecyclerView recyclerView;
  private KyBaseRecyclerAdapter<CloudSuperArea> recyclerAdapter;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.activity_trial);
    context = this;
    trialActivity_toolbar = (KyToolBar) findViewById(R.id.trialActivity_kyToolbar);
    recyclerView = (RecyclerView) findViewById(R.id.trialActivity_recyclerView);
    if (recyclerAdapter == null) initRecyclerAdapter(new ArrayList<CloudSuperArea>());
    recyclerView.setAdapter(recyclerAdapter);
    recyclerView.setLayoutManager(new GridLayoutManager(context, 2));
    recyclerView.setItemAnimator(new DefaultItemAnimator());
    if (recyclerAdapter.getItemCount() == 0) {
      reloadCloudSuperAreaList();
    }

    trialActivity_toolbar.setOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        finish();
      }
    });
  }

  private void reloadCloudSuperAreaList() {
    List<CloudSuperArea> cloudSuperAreas = new ArrayList<>();
    cloudSuperAreas.add(new CloudSuperArea("桂电草坪", "http://oqza83elq.bkt.clouddn.com/super3.jpg", true));
    recyclerAdapter.reloadData(cloudSuperAreas);
  }
  void initRecyclerAdapter(List<CloudSuperArea> list) {
    recyclerAdapter =
        new KyBaseRecyclerAdapter<CloudSuperArea>(list, R.layout.recycler_super_area_show) {
          @Override
          public void setDataToViewHolder(final CloudSuperArea dataItem, KyBaseViewHolder holder) {
            holder.getSimpleDraweeView(R.id.recycler_super_area_show_draweeview)
                .setImageURI(Uri.parse(dataItem.getPhotoUri()));
            holder.getTextView(R.id.recycler_super_area_show_text_title).setText(dataItem.getName());
           }
        };
    recyclerAdapter.setOnClickListener(
        new KyBaseRecyclerAdapter.KyRecyclerViewItemOnClickListener<CloudSuperArea>() {
          @Override
          public void Onclick(View v, int adapterPosition, CloudSuperArea data) {
            Intent intent = new Intent(context, NodeControlActivity.class);
            intent.putExtra("areaName", data.getName());
            startActivity(intent);
          }
        });
  }
}
