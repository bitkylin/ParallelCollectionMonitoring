package cc.bitky.warningsystem.fragment;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.DefaultItemAnimator;
import android.support.v7.widget.GridLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.ArrayList;
import java.util.List;

import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudSuperArea;
import cc.bitky.warningsystem.trial.TrialActivity;
import cc.bitky.warningsystem.utils.KyBaseRecyclerAdapter;
import cc.bitky.warningsystem.utils.KyBaseViewHolder;

public class RecyclerFragment extends Fragment {

  RecyclerView recyclerView;
  private KyBaseRecyclerAdapter<CloudSuperArea> recyclerAdapter;
  private View view;
  Context context;

  public RecyclerFragment() {
    // Required empty public constructor
  }

  @Override
  public void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    context = getContext();

  }

  @Override
  public View onCreateView(LayoutInflater inflater, ViewGroup container,
                           Bundle savedInstanceState) {
    view = inflater.inflate(R.layout.fragment_recycler, container, false);
    initRecyclerView(view);
    return view;
  }

  @Override
  public void onAttach(Context context) {
    super.onAttach(context);
  }

  /**
   * 初始化RecyclerView
   */
  private void initRecyclerView(View view) {
    //商品RecyclerView显示
    recyclerView = (RecyclerView) view.findViewById(R.id.mainActivity_recyclerView);
    if (recyclerAdapter == null) initRecyclerAdapter(new ArrayList<CloudSuperArea>());
    recyclerView.setAdapter(recyclerAdapter);
    recyclerView.setLayoutManager(new GridLayoutManager(context, 2));
    recyclerView.setItemAnimator(new DefaultItemAnimator());
    if (recyclerAdapter.getItemCount() == 0) {
      reloadCloudSuperAreaList();
    }
  }

  private void reloadCloudSuperAreaList() {
    List<CloudSuperArea> cloudSuperAreas = new ArrayList<>();
    cloudSuperAreas.add(new CloudSuperArea("桂林电子科技大学", "http://oqza83elq.bkt.clouddn.com/super9.jpg", true));
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
            if (dataItem.isStatus())
              holder.getView(R.id.recycler_super_area_show_layout).setBackground(getResources().getDrawable(R.drawable.selector_list_item_normal));
            else
              holder.getView(R.id.recycler_super_area_show_layout).setBackground(getResources().getDrawable(R.drawable.selector_list_item_warn));
          }
        };
    recyclerAdapter.setOnClickListener(
        new KyBaseRecyclerAdapter.KyRecyclerViewItemOnClickListener<CloudSuperArea>() {
          @Override
          public void onclick(View v, int adapterPosition, CloudSuperArea data) {
            Intent intent = new Intent(context, TrialActivity.class);
            intent.putExtra("areaName", data.getName());
            startActivity(intent);
          }
        });
  }
}
