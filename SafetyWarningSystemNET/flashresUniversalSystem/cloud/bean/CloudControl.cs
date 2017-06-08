using System;
using cn.bmob.io;

namespace bitkyFlashresUniversal.cloud.bean
{
    public class CloudControl : BmobTable
    {
        public BmobInt cloudProcessProgress { set; get; }
        public BmobBoolean cloudProcessOpened { set; get; }
        public BmobBoolean deployProcessOpened { set; get; }
        public BmobInt cloudCollectProgress { set; get; }
        public BmobBoolean cloudCollectOpened { set; get; }
        public BmobBoolean deployCollectOpened { set; get; }

        //读字段信息
        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            cloudProcessProgress = input.getInt("cloudProcessProgress");
            cloudCollectProgress = input.getInt("cloudCollectProgress");
            cloudProcessOpened = input.getBoolean("cloudProcessOpened");
            deployProcessOpened = input.getBoolean("deployProcessOpened");
            cloudCollectOpened = input.getBoolean("cloudCollectOpened");
            deployCollectOpened = input.getBoolean("deployCollectOpened");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("cloudProcessProgress", cloudProcessProgress);
            output.Put("cloudProcessOpened", cloudProcessOpened);
            output.Put("deployProcessOpened", deployProcessOpened);
            output.Put("cloudCollectProgress", cloudCollectProgress);
            output.Put("cloudCollectOpened", cloudCollectOpened);
            output.Put("deployCollectOpened", deployCollectOpened);
        }
    }
}