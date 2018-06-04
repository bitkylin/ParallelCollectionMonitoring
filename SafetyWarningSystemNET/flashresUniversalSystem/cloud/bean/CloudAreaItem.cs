using cn.bmob.io;
using System;

namespace bitkyFlashresUniversal.cloud.bean
{
    public class CloudAreaItem : BmobTable
    {
        /**
         * 地点名
         */
        public String name { get; set; }
        /**
         * 状态详情
         */
        public String detail { get; set; }
        /**
         * 当前监测状态
         */
        public BmobBoolean status { get; set; }
        public BmobBoolean enabled { get; set; }
        /**
         * 生成时间
         */
        public BmobDate time { get; set; }
        /**
         * 图片地址
         */
        public String photoUri { get; set; }
        /**
         * 反演进度
         */
        public BmobInt processBar { get; set; }

        public BmobInt processStatus { get; set; }

        public String coordinate { get; set; }
        //读字段信息
        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            name = input.getString("name");
            detail = input.getString("detail");
            status = input.getBoolean("status");
            enabled = input.getBoolean("enabled");
            time = input.getDate("time");
            photoUri = input.getString("photoUri");
            processBar = input.getInt("processBar");
            processStatus = input.getInt("processStatus");
            coordinate = input.getString("coordinate");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("name", name);
            output.Put("detail", detail);
            output.Put("status", status);
            output.Put("enabled", enabled);
            output.Put("time", time);
            output.Put("photoUri", photoUri);
            output.Put("processBar", processBar);
            output.Put("processStatus", processStatus);
            output.Put("coordinate", coordinate);
        }
    }
}