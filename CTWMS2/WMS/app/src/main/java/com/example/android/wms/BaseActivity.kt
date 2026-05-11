package com.example.android.wms

import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.util.Log
import android.view.MotionEvent
import androidx.appcompat.app.AppCompatActivity
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.ViewData.ViewMenuActivity

open class BaseActivity: AppCompatActivity() {

    var countTimerView : CountTimer? = null
    var countTimerView2 : CountTimer2? = null
    public var login : Boolean = true

    @Override
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        init();
        countTimerView2?.start()
    }

    private fun init() {
        //初始化CountTimer，设置倒计时为2分钟。
        countTimerView = CountTimer(5000, 1000)
        countTimerView2 = CountTimer2(5000, 1000)
    }


    private fun timeStart() {
        Handler(getMainLooper()).post(Runnable() {
            fun run() {
                countTimerView?.start()
            }
        })
    }

    /**
     * 主要的方法，重写dispatchTouchEvent
     *
     * @param event
     * @return
     */
    @Override
    override fun dispatchTouchEvent(event: MotionEvent): Boolean {
        when  (event.getAction()) {
            //获取触摸动作，如果ACTION_UP，计时开始。
            MotionEvent.ACTION_UP ->
            countTimerView?.start()
            //break
            //否则其他动作计时取消
            else -> {
                countTimerView?.cancel()
                //break
            }
        }
        return super.dispatchTouchEvent(event)
    }

    override fun onResume() {
        super.onResume();
        timeStart();
    }

    override fun onPause() {
        super.onPause();
        countTimerView?.cancel();
        countTimerView2?.cancel();
    }
}