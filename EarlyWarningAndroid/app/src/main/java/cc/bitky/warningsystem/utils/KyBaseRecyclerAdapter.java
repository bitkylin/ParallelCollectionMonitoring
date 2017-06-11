package cc.bitky.warningsystem.utils;

import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.ArrayList;
import java.util.List;

public abstract class KyBaseRecyclerAdapter<T> extends RecyclerView.Adapter<KyBaseViewHolder> {
  protected KyRecyclerViewItemOnClickListener listener;
  protected List<T> mDatas = new ArrayList<>();
  protected int resourceId;

  /**
   * 初始化 RecyclerView 适配器
   *
   * @param mDatas     绑定的数据
   * @param resourceId 绑定的ViewItem
   */
  public KyBaseRecyclerAdapter(List<T> mDatas, int resourceId) {
    this.mDatas = mDatas;
    this.resourceId = resourceId;
  }

  /**
   * 设置RecyclerView中条目的点击监听器
   *
   * @param listener 设置点击监听器
   */
  public void setOnClickListener(KyRecyclerViewItemOnClickListener listener) {
    this.listener = listener;
  }

  @Override
  public KyBaseViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
    View view = LayoutInflater.from(parent.getContext()).inflate(resourceId, parent, false);
    return new KyBaseViewHolder(view, new KyBaseViewHolder.HolderOnClickListener() {
      @Override
      public void Onclick(View v, int adapterPosition) {
        if (listener != null) listener.Onclick(v, adapterPosition, mDatas.get(adapterPosition));
      }
    });
  }

  @Override
  public void onBindViewHolder(KyBaseViewHolder holder, int position) {
    T dataItem = mDatas.get(position);
    setDataToViewHolder(dataItem, holder);
  }

  /**
   * 在ViewHolder中绑定数据
   *
   * @param dataItem 数据bean
   * @param holder   绑定数据的ViewHolder
   */
  public abstract void setDataToViewHolder(T dataItem, KyBaseViewHolder holder);

  @Override
  public int getItemCount() {
    return mDatas.size();
  }

  public List<T> getDataItems() {
    return mDatas;
  }

  public void reloadData(List<T> list) {
    if (mDatas != null) {
      int count = mDatas.size();
      mDatas.clear();
      notifyItemRangeRemoved(0, count);
      mDatas.addAll(list);
      notifyItemRangeChanged(0, list.size());
    }
  }

  public void loadMoreData(List<T> list) {
    int count = mDatas.size();
    mDatas.addAll(list);
    notifyItemRangeChanged(count, list.size());
  }

  public interface KyRecyclerViewItemOnClickListener<T> {
    void Onclick(View v, int adapterPosition, T data);
  }
}
