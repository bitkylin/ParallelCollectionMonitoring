using System;
using cn.bmob.io;

namespace bitkyFlashresUniversal.cloud.bean
{
    public class CloudDevice : BmobTable
    {

        public BmobBoolean enabled { set; get; }
        public BmobInt status { set; get; }

        //读字段信息
        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            enabled = input.getBoolean("enabled");
            status = input.getInt("status");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);
            
            output.Put("enabled", enabled);
            output.Put("status", status);
        }
    }
}