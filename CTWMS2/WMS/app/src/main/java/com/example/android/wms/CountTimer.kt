package com.example.android.wms


import android.content.Context
import android.content.Intent
import android.os.CountDownTimer
import android.util.Log
import com.example.android.wms.Login.LoginActivity


class CountTimer(millisInFuture: Long, countDownInterval: Long) : CountDownTimer(millisInFuture,
    countDownInterval) {

    //计时过程显示：如有需求你可以在特定的时间范围内执行自己操作
    override fun onTick(p0: Long) {
        Log.e("tag", p0.toString());
    }

    //计时完毕时触发：我们的需求中的实现逻辑在这里，一般都逻辑会跳转一个屏保的Activity
    override fun onFinish() {
        Log.i("tag", "Finish");
        start()
        //TODO("Not yet implemented")
    }
}

class CountTimer2(millisInFuture: Long, countDownInterval: Long) : CountDownTimer(millisInFuture,
    countDownInterval) {

    var Timeout : Boolean = false

    //计时过程显示：如有需求你可以在特定的时间范围内执行自己操作
    override fun onTick(p0: Long) {
        Log.e("tag", "123" + p0.toString());
    }

    //计时完毕时触发：我们的需求中的实现逻辑在这里，一般都逻辑会跳转一个屏保的Activity
    override fun onFinish() {
        Log.i("tag", "Finish");
        Timeout  = true
        //TODO("Not yet implemented")
    }
    fun CheckTimeout() :Boolean
    {
        return Timeout
    }
}