package cc.bitky.warningsystem.utils;

import android.support.v7.widget.RecyclerView;
import android.util.SparseArray;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.TextView;

import com.facebook.drawee.view.SimpleDraweeView;

public class KyBaseViewHolder extends RecyclerView.ViewHolder {
    SparseArray<View> sparseArray;
    View itemView;

    public KyBaseViewHolder(View itemView, final HolderOnClickListener listener) {
        super(itemView);
        sparseArray = new SparseArray<>();
        this.itemView = itemView;
        if (listener != null) {
            itemView.setOnClickListener(v -> listener.onclick(v, getAdapterPosition()));
        }
    }

    public <T extends View> T getView(int id) {
        View view = sparseArray.get(id);
        if (view == null) {
            view = itemView.findViewById(id);
            sparseArray.append(id, view);
        }
        return (T) view;
    }

    public SimpleDraweeView getSimpleDraweeView(int id) {
        return getView(id);
    }

    public TextView getTextView(int id) {
        return getView(id);
    }

    public Button getButton(int id) {
        return getView(id);
    }

    public CheckBox getCheckBox(int id) {
        return getView(id);
    }

    interface HolderOnClickListener {
        void onclick(View v, int adapterPosition);
    }
}