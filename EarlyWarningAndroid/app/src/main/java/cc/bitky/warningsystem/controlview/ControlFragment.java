package cc.bitky.warningsystem.controlview;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.util.Random;

import cc.bitky.warningsystem.R;
import cc.bitky.warningsystem.bean.CloudControl;
import cc.bitky.warningsystem.trial.control.DetailActivity;
import cn.bmob.v3.BmobQuery;
import cn.bmob.v3.exception.BmobException;
import cn.bmob.v3.listener.QueryListener;
import cn.bmob.v3.listener.UpdateListener;


public class ControlFragment extends Fragment implements View.OnClickListener {
  private Button nodeControlActivity_btnCollect;
  private Button nodeControlActivity_btnProcess;
  private TextView nodeControlActivity_textCollectStatus;
  private TextView nodeControlActivity_textProcessStatus;
  private ProgressBar nodeControlActivity_progressBarCollect;
  private ProgressBar nodeControlActivity_progressBarProcess;
  private TextView nodeControlActivity_nodeStatus;
  private Button nodeControlActivity_btnResult;

  private int processValue = 0;
  Random random = new Random();
  Context context;
  boolean ThreadStoped = false;

  @Override
  public void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    context = getActivity();
  }

  @Override
  public View onCreateView(LayoutInflater inflater, ViewGroup container,
                           Bundle savedInstanceState) {
    NodeControlActivity activity = (NodeControlActivity) getActivity();
    View view = inflater.inflate(R.layout.fragment_control, container, false);
    TextView nodeControlFragment_textTitle = (TextView) view.findViewById(R.id.nodeControlFragment_textTitle);
    nodeControlFragment_textTitle.setText(activity.areaName);
    nodeControlActivity_nodeStatus = (TextView) view.findViewById(R.id.nodeControlFragment_nodeStatus);
    nodeControlActivity_btnCollect = (Button) view.findViewById(R.id.nodeControlFragment_btnCollect);
    nodeControlActivity_textCollectStatus = (TextView) view.findViewById(R.id.nodeControlFragment_textCollectStatus);
    nodeControlActivity_progressBarCollect = (ProgressBar) view.findViewById(R.id.nodeControlFragment_progressBarCollect);
    nodeControlActivity_btnProcess = (Button) view.findViewById(R.id.nodeControlFragment_btnProcess);
    nodeControlActivity_textProcessStatus = (TextView) view.findViewById(R.id.nodeControlFragment_textProcessStatus);
    nodeControlActivity_progressBarProcess = (ProgressBar) view.findViewById(R.id.nodeControlFragment_progressBarProcess);
    nodeControlActivity_btnResult = (Button) view.findViewById(R.id.nodeControlFragment_btnResult);

    nodeControlActivity_btnResult.setOnClickListener(this);
    nodeControlActivity_btnProcess.setOnClickListener(this);
    nodeControlActivity_btnCollect.setOnClickListener(this);

    ThreadStoped = false;
    new Thread(new Runnable() {
      @Override
      public void run() {
        while (!ThreadStoped) {
          queryDataFromBmob();
          try {
            Thread.sleep(3000);
          } catch (InterruptedException ignored) {

          }
          if (processValue < 100) {
            if (processValue < 100) processValue += random.nextInt(25);
            if (processValue >= 100) processValue = 100;
          }
        }
      }
    }).start();


    return view;
  }

  @Override
  public void onResume() {
    super.onResume();
    ThreadStoped = false;
  }

  @Override
  public void onPause() {
    super.onPause();
    ThreadStoped = true;
  }

  private void queryDataFromBmob() {
    BmobQuery<CloudControl> kyOperationBmobQuery = new BmobQuery<>();
    kyOperationBmobQuery.getObject("W2obDDDL", new QueryListener<CloudControl>() {
      @Override
      public void done(CloudControl cloudControl, BmobException e) {
        Log.d("lml", "done: 查询一次");
        if (e != null) {
          Log.d("lml", e.getMessage());
          return;
        }
        nodeControlActivity_nodeStatus.setText("正常");
        if (cloudControl.getCloudCollectOpened()) {
          nodeControlActivity_btnResult.setEnabled(false);
          int progress = cloudControl.getCloudCollectProgress();
          if (progress < 100) {
            nodeControlActivity_textCollectStatus.setText("正在采集");
            nodeControlActivity_btnCollect.setEnabled(false);
          } else {
            nodeControlActivity_textCollectStatus.setText("采集完成");
            nodeControlActivity_btnCollect.setEnabled(true);
            nodeControlActivity_btnProcess.setEnabled(true);
          }
          nodeControlActivity_progressBarCollect.setProgress(cloudControl.getCloudCollectProgress());
        } else {
          nodeControlActivity_textCollectStatus.setText("未采集");
          nodeControlActivity_btnCollect.setEnabled(true);
        }

        if (cloudControl.getDeployProcessOpened()) {
          nodeControlActivity_btnCollect.setEnabled(false);
          nodeControlActivity_btnProcess.setEnabled(false);
          int progress = processValue;//cloudControl.getCloudProcessProgress();
          if (progress < 100)
            nodeControlActivity_textProcessStatus.setText("正在反演");
          else {
            nodeControlActivity_textProcessStatus.setText("反演完成");
            nodeControlActivity_btnCollect.setEnabled(true);
            nodeControlActivity_btnResult.setEnabled(true);
          }
          nodeControlActivity_progressBarProcess.setProgress(progress);
        } else {
          nodeControlActivity_textProcessStatus.setText("未反演");
        }
      }
    });
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
  public void onClick(View v) {
    CloudControl cloudControl = new CloudControl();
    switch (v.getId()) {
      case R.id.nodeControlFragment_btnCollect:
        cloudControl.setObjectId("W2obDDDL");
        cloudControl.setDeployCollectOpened(true);
        cloudControl.setDeployProcessOpened(false);
        cloudControl.setCloudCollectProgress(0);
        cloudControl.setCloudProcessOpened(false);
        cloudControl.setCloudProcessProgress(0);
        processValue = 0;
        cloudControl.update(new UpdateListener() {
          @Override
          public void done(BmobException e) {
            if (e != null) Log.d("lml", "done: " + e.getMessage());
            else {
              nodeControlActivity_progressBarCollect.setProgress(0);
              nodeControlActivity_progressBarProcess.setProgress(0);
            }
          }
        });
        break;
      case R.id.nodeControlFragment_btnProcess:
        cloudControl.setObjectId("W2obDDDL");
        cloudControl.setDeployCollectOpened(false);
        cloudControl.setDeployProcessOpened(true);
        cloudControl.update(new UpdateListener() {
          @Override
          public void done(BmobException e) {
            if (e != null) Log.d("lml", "done: " + e.getMessage());
            else processValue = 0;
          }
        });
        break;
      case R.id.nodeControlFragment_btnResult:
        Intent intent = new Intent(context, DetailActivity.class);
        intent.putExtra("objectId", "Cxq8P77P");
        startActivity(intent);
        break;
    }
  }
}
