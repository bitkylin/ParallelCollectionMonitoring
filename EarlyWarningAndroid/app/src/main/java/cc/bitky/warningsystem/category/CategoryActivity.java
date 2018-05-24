package cc.bitky.warningsystem.category;

import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.DefaultItemAnimator;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.facebook.drawee.view.SimpleDraweeView;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.List;

import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudAreaItem;
import cc.bitky.warningsystem.utils.KyBaseRecyclerAdapter;
import cc.bitky.warningsystem.utils.KyBaseViewHolder;
import cc.bitky.warningsystem.utils.KyToolBar;
import cn.bmob.v3.BmobQuery;
import cn.bmob.v3.exception.BmobException;
import cn.bmob.v3.listener.FindListener;
import cn.bmob.v3.listener.QueryListener;

public class CategoryActivity extends AppCompatActivity {

    private KyBaseRecyclerAdapter<CloudAreaItem> recyclerAdapterCategoryStr;
    private CategoryPresenter presenter;
    private TextView categoryActivity_textStatus;
    private TextView categoryActivity_textTime;
    private TextView categoryActivity_textTitle;
    private TextView categoryActivity_prepare_textshow;
    private KyToolBar categoryActivity_toolbar;

    private DateFormat dateFormat = SimpleDateFormat.getDateTimeInstance();
    private String TAG = "lml";
    private String areaName = null;
    private SimpleDraweeView categoryActivity_draweeview;
    private LinearLayout categoryActivity_Layout_prepare;
    private LinearLayout categoryActivity_Layout_show;
    private ProgressBar categoryActivity_processbar;
    private TextView categoryActivity_textDetail;
    private TextView categoryActivity_coordinate;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_category);
        initView();
        areaName = getIntent().getStringExtra("areaName");
        if (areaName == null) areaName = "三峡大坝";
        categoryActivity_toolbar.setTitle(areaName);
        categoryActivity_toolbar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });
        presenter = new CategoryPresenter(this);
        initRecyclerView();
        initListByName(areaName, true);
    }

    private void initListByName(final String areaName, final boolean initView) {
        BmobQuery<CloudAreaItem> bmobQuery = new BmobQuery<>();
        bmobQuery.addWhereEqualTo("name", areaName);
        bmobQuery.addWhereEqualTo("enabled", true);
        bmobQuery.order("time");
        bmobQuery.findObjects(new FindListener<CloudAreaItem>() {
            @Override
            public void done(List<CloudAreaItem> object, BmobException e) {
                if (e == null) {
                    Log.d(TAG, "done: " + object.size());
                    recyclerAdapterCategoryStr.reloadData(object);
                    if (object.size() == 0) {
                        categoryActivity_prepare_textshow.setText("未获取到数据");
                    } else if (initView) {
                        updateDetailData(object.get(object.size() - 1));
                    }
                    //注意：这里的Person对象中只有指定列的数据。
                } else {
                    Log.i("bmob", "失败：" + e.getMessage() + "," + e.getErrorCode());
                }
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

    private void initView() {
        categoryActivity_toolbar = (KyToolBar) findViewById(R.id.categoryActivity_toolbar);
        categoryActivity_Layout_prepare = (LinearLayout) findViewById(R.id.categoryActivity_Layout_prepare);
        categoryActivity_Layout_show = (LinearLayout) findViewById(R.id.categoryActivity_Layout_show);
        categoryActivity_textTitle = (TextView) findViewById(R.id.categoryActivity_textTitle);
        categoryActivity_prepare_textshow = (TextView) findViewById(R.id.categoryActivity_prepare_textshow);
        categoryActivity_processbar = (ProgressBar) findViewById(R.id.categoryActivity_processbar);
        categoryActivity_textStatus = (TextView) findViewById(R.id.categoryActivity_textStatus);
        categoryActivity_coordinate = (TextView) findViewById(R.id.categoryActivity_coordinate);
        categoryActivity_textTime = (TextView) findViewById(R.id.categoryActivity_textTime);
        categoryActivity_textDetail = (TextView) findViewById(R.id.categoryActivity_textDetail);
        categoryActivity_draweeview = (SimpleDraweeView) findViewById(R.id.categoryActivity_draweeview);
    }

    /**
     * 初始化RecyclerView
     */
    private void initRecyclerView() {
        //分类RecyclerView显示
        RecyclerView recyclerViewCategory = findViewById(R.id.categoryActivity_recyclerview_category);
        recyclerViewCategory.setAdapter(getRecyclerAdapterCategoryStr());
        recyclerViewCategory.setLayoutManager(new LinearLayoutManager(this));
        recyclerViewCategory.setItemAnimator(new DefaultItemAnimator());
    }

    private KyBaseRecyclerAdapter getRecyclerAdapterCategoryStr() {
        if (recyclerAdapterCategoryStr == null) {
            List<CloudAreaItem> categrayNames = presenter.getCategoryNames();
            recyclerAdapterCategoryStr = new KyBaseRecyclerAdapter<CloudAreaItem>(categrayNames,
                    R.layout.recycler_categryfragment_single_text) {
                @Override
                public void setDataToViewHolder(CloudAreaItem dataItem, KyBaseViewHolder holder) {
                    holder.getTextView(R.id.recycler_categry_single_textView).setText(dataItem.getTime().getDate());
                    if (dataItem.getProcessStatus() < 3)
                        holder.getTextView(R.id.recycler_categry_status).setBackgroundColor(getResources().getColor(R.color.lightslategray));
                    else if (dataItem.getStatus())
                        holder.getTextView(R.id.recycler_categry_status).setBackgroundColor(getResources().getColor(R.color.lime));
                    else
                        holder.getTextView(R.id.recycler_categry_status).setBackgroundColor(getResources().getColor(R.color.red));
                }
            };
            recyclerAdapterCategoryStr.setOnClickListener(
                    new KyBaseRecyclerAdapter.KyRecyclerViewItemOnClickListener<CloudAreaItem>() {
                        @Override
                        public void onclick(View v, int adapterPosition, CloudAreaItem data) {
                            categoryActivity_Layout_prepare.setVisibility(View.VISIBLE);
                            categoryActivity_Layout_show.setVisibility(View.GONE);
                            categoryActivity_prepare_textshow.setText("正在获取数据，请稍候 ...");
                            categoryActivity_processbar.setVisibility(View.GONE);
                            updateDetailShow(data.getObjectId());
                            initListByName(areaName, false);
                        }
                    });
        }
        return recyclerAdapterCategoryStr;
    }

    public void updateDetailData(CloudAreaItem data) {

        if (data.getProcessStatus() == 3) {
            categoryActivity_Layout_prepare.setVisibility(View.GONE);
            categoryActivity_Layout_show.setVisibility(View.VISIBLE);
            categoryActivity_textDetail.setText(data.getDetail());
            categoryActivity_textTitle.setText(areaName);
            if (data.getStatus()) {
                categoryActivity_textStatus.setText("正常");
                categoryActivity_textStatus.setTextColor(getResources().getColor(R.color.black));

            } else {
                categoryActivity_textStatus.setText("异常");
                categoryActivity_textStatus.setTextColor(getResources().getColor(R.color.red));
            }
            categoryActivity_textTime.setText(data.getTime().getDate());
            categoryActivity_draweeview.setImageURI(data.getPhotoUri());
            categoryActivity_coordinate.setText(data.getCoordinate());
        } else {
            if (data.getProcessStatus() == 2) {
                categoryActivity_prepare_textshow.setText("正在进行正反演分析 ...");
            } else {
                categoryActivity_prepare_textshow.setText("正在进行数据的采集与整理，请稍候 ...");
            }
            categoryActivity_Layout_prepare.setVisibility(View.VISIBLE);
            categoryActivity_Layout_show.setVisibility(View.GONE);
            categoryActivity_processbar.setVisibility(View.VISIBLE);
            categoryActivity_processbar.setProgress(data.getProcessBar());
        }
    }
}
