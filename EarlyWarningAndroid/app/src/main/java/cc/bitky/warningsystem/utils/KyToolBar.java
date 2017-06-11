package cc.bitky.warningsystem.utils;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.support.annotation.Nullable;
import android.support.v7.widget.TintTypedArray;
import android.support.v7.widget.Toolbar;
import android.util.AttributeSet;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;

import cc.bitky.warningsystem.R;

public class KyToolBar extends Toolbar {
  private TextView textView;
  private View view;
  private ImageView kyNavigation;
  private ImageView rightButton;
  private TextView rightTextView;
  private TextView placeHolderTextView;
  private EditText editText;
  private OnSearchListener onSearchListener;

  public KyToolBar(Context context) {
    this(context, null);
  }

  public KyToolBar(Context context, @Nullable AttributeSet attrs) {
    this(context, attrs, 0);
  }

  public KyToolBar(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
    super(context, attrs, defStyleAttr);

    initView();
    final TintTypedArray a =
        TintTypedArray.obtainStyledAttributes(getContext(), attrs, R.styleable.KyToolbar,
            defStyleAttr, 0);
    final boolean enableKyNavigationIcon =
        a.getBoolean(R.styleable.KyToolbar_enableKyNavigationIcon, true);
    final boolean enableKySearch = a.getBoolean(R.styleable.KyToolbar_enableKySearch, false);
    final Drawable navIcon = a.getDrawable(R.styleable.KyToolbar_KyNavigationIcon);
    final Drawable rightButtonIcon = a.getDrawable(R.styleable.KyToolbar_setRightButton);
    final String rightText = a.getString(R.styleable.KyToolbar_setRightText);

    setNavigationIcon(navIcon);
    setRightButtonIcon(rightButtonIcon);
    enableKyNavigation(enableKyNavigationIcon);
    setRightTextView(rightText);
    setEnabledSearch(enableKySearch);
    a.recycle();
  }

  private void initView() {
    LayoutInflater inflater = LayoutInflater.from(getContext());
    view = inflater.inflate(R.layout.widget_ky_toolbar, this, true);
    textView = (TextView) view.findViewById(R.id.kytoolbar_title);
    editText = (EditText) view.findViewById(R.id.kytoolbar_editText);
    kyNavigation = (ImageView) view.findViewById(R.id.kytoolbar_navigation);
    rightButton = (ImageView) view.findViewById(R.id.kytoolbar_rightButton);
    rightTextView = (TextView) view.findViewById(R.id.kytoolbar_rightTextView);
    placeHolderTextView = (TextView) view.findViewById(R.id.kytoolbar_placeholder);
  }

  @Override public void setNavigationOnClickListener(OnClickListener listener) {
    kyNavigation.setOnClickListener(listener);
  }

  public void setRightButtonOnClickListener(OnClickListener listener) {
    rightButton.setOnClickListener(listener);
  }

  public void setRightTextViewOnClickListener(OnClickListener listener) {
    rightTextView.setOnClickListener(listener);
  }

  /**
   * 是否启用kyNavigation
   *
   * @param enableKyNavigationIcon kyNavigation
   */
  private void enableKyNavigation(boolean enableKyNavigationIcon) {
    if (enableKyNavigationIcon) {
      kyNavigation.setVisibility(VISIBLE);
    } else {
      kyNavigation.setVisibility(INVISIBLE);
    }
  }

  /**
   * 是否启用 rightButton
   *
   * @param enableRightButton kyNavigation
   */
  private void enableRightButton(boolean enableRightButton) {
    if (enableRightButton) {
      rightButton.setVisibility(VISIBLE);
    } else {
      rightButton.setVisibility(GONE);
    }
  }

  /**
   * 是否启用 RightTextView
   *
   * @param enableRightTextView enableRightTextView
   */
  private void enableRightTextView(boolean enableRightTextView) {
    if (enableRightTextView) {
      rightTextView.setVisibility(VISIBLE);
    } else {
      rightTextView.setVisibility(GONE);
    }
  }

  /**
   * 设置 navigation 的图标
   *
   * @param icon 图标
   */
  @Override public void setNavigationIcon(@Nullable Drawable icon) {
    if (icon != null) {
      kyNavigation.setImageDrawable(icon);
      enableKyNavigation(true);
    }
  }

  /**
   * 设置 RightButton 的图标
   *
   * @param icon 图标
   */
  private void setRightButtonIcon(@Nullable Drawable icon) {
    if (icon != null) {
      rightButton.setImageDrawable(icon);
      enableRightButton(true);
      placeHolderTextView.setVisibility(GONE);
    }
  }

  /**
   * 设置 RightTextView 启用
   *
   * @param str 显示的文字
   */
  private void setRightTextView(@Nullable String str) {
    if (str != null) {
      rightTextView.setText(str);
      enableRightTextView(true);
      placeHolderTextView.setVisibility(GONE);
    }
  }

  @Override public void setTitle(CharSequence title) {
    super.setTitle(title);
    if (view == null) initView();
    textView.setText(title);
  }

  /**
   * 设置是否启用搜索
   */
  public void setEnabledSearch(boolean enabledSearch) {
    if (enabledSearch) {
      textView.setVisibility(GONE);
      editText.setVisibility(VISIBLE);
      editText.setOnEditorActionListener(new TextView.OnEditorActionListener() {
        @Override public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
          if (actionId == EditorInfo.IME_ACTION_SEARCH && onSearchListener != null) {
            onSearchListener.searchListener(editText.getText().toString().trim());
            return true;
          }
          return false;
        }
      });
    }
  }

  public void setOnSearchListener(OnSearchListener onSearchListener) {
    this.onSearchListener = onSearchListener;
  }

  public interface OnSearchListener {
    void searchListener(String msg);
  }
}

