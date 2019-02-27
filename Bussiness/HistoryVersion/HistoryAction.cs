using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.HistoryVersion
{
    public class HistoryAction : BaseAction, ICompanyAction
    {
        public void Start()
        {
            Console.WriteLine(Log());
            Console.ReadLine();
        }
        private string Log()
        {
            return @"
V1.0.0.11更新内容：
营业有偿开票申请-采购数据、开票申请-销售、开票申请冲销-采购、开票申请冲销-销售
V1.0.0.10更新内容：
添加SAP凭证回传
V1.0.0.9更新内容：
修改公司经费Head文本，追加财务初审审核人
V1.0.0.8更新内容：
个人经费添加DCT、DCSH
V1.0.0.7更新内容：
修复BinYi_Synchor的DICG联动
V1.0.0.6更新内容：
添加DSCS凭证联动
V1.0.0.5更新内容：
修复订单号出现-后截取错误
V1.0.0.4更新内容：
添加DCBJ凭证生成
V1.0.0.3更新内容：
修复公司经费付款通知书支付、销账，暂收款在多申请单下，一个单号每条明细生成一个贷方科目
V1.0.0.2更新内容：
1.PAY支付回传会发送凭证回传报错邮件
2.SAP凭证回传、PAY回传成功失败路径自动添加\判断(SAPToBPMResultQueue中最后的\自动判断是否添加)
3.SAP凭证连携不生成空文件
V1.0.0.1更新内容：
1.添加个人经费：DCGZ
2.添加SAP2数据回传，包含：供应商回传(个人P开头)、成本中心回传、PAY回传
            ";
        }
    }
}
