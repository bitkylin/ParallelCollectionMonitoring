package cc.bitky.warningsystem.controlview;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.TimePicker;

import java.util.Calendar;
import java.util.TimerTask;

import cc.bitky.warningsystem.R;

public class OptionFragment extends Fragment {

  Calendar calendar;
  TextView optionFragment_tvTime;
  Context context;

  @Override
  public void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    context = getContext();
    calendar = Calendar.getInstance();
  }

  @Override
  public View onCreateView(final LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
    View view = inflater.inflate(R.layout.fragment_option, container, false);
    NodeControlActivity activity = (NodeControlActivity) getActivity();
    TextView nodeControlFragment_textTitle = (TextView) view.findViewById(R.id.optionFragment_textTitle);
    nodeControlFragment_textTitle.setText(activity.areaName);
    optionFragment_tvTime = (TextView) view.findViewById(R.id.optionFragment_tvTime);
    optionFragment_tvTime.setOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        pickerTime(inflater);
      }
    });
    view.findViewById(R.id.optionFragment_btnConfirmConfig).setOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View v) {
        AlertDialog builder = new AlertDialog.Builder(context)
            .setTitle("设置成功")
            .setMessage("当前设定：每天 " + optionFragment_tvTime.getText() + "\n进行数据采集任务").create();
        builder.show();
      }
    });
    return view;
  }

  private void pickerTime(LayoutInflater inflater) {
    //自定义控件
    AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
    View viewTime = inflater.inflate(R.layout.dialog_setting_time, null);
    final TimePicker timePicker = (TimePicker) viewTime.findViewById(R.id.time_picker);
    //初始化时间
    calendar.setTimeInMillis(System.currentTimeMillis());
    timePicker.setIs24HourView(true);
    timePicker.setCurrentHour(calendar.get(Calendar.HOUR_OF_DAY));
    timePicker.setCurrentMinute(calendar.get(Calendar.MINUTE));
    //设置time布局
    builder.setView(viewTime);
    builder.setPositiveButton("确定", new DialogInterface.OnClickListener() {
      @Override
      public void onClick(DialogInterface dialog, int which) {
        Log.d("lml", "时间输出：" + timePicker.getCurrentHour() + "  " + timePicker.getCurrentMinute());
        getActivity().runOnUiThread(new TimerTask() {
          @Override
          public void run() {
            if (timePicker.getCurrentMinute() < 10)
              optionFragment_tvTime.setText(timePicker.getCurrentHour() + " : 0" + timePicker.getCurrentMinute());
            else
              optionFragment_tvTime.setText(timePicker.getCurrentHour() + " : " + timePicker.getCurrentMinute());
          }
        });
        dialog.cancel();
      }
    });
    builder.setNegativeButton("取消", new DialogInterface.OnClickListener() {
      @Override
      public void onClick(DialogInterface dialog, int which) {
        dialog.cancel();
      }
    });
    builder.create().show();
  }

  @Override
  public void onAttach(Context context) {
    super.onAttach(context);
  }

  @Override
  public void onDetach() {
    super.onDetach();
  }
}
