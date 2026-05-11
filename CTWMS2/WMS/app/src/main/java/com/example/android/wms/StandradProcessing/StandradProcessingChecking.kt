package com.example.android.wms.StandradProcessing

import android.graphics.Color
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.SupervisorJob

class StandradProcessingChecking : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_standrad_processing_checking)

        val applicationScope = CoroutineScope(SupervisorJob())
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
        val repository = StandradProcessingRepository(dao)
        val factory = StandradProcessingViewModel(repository, application)
        val standradprocessingViewModel: StandradProcessingViewModel = factory


        //val pl1 = standradprocessingViewModel.GetPL3()
        //val pi1 = standradprocessingViewModel.getInnerCarton3()
        //val pi2 = pi1

        // if ((p0 <= pl.size)&&(pl.size != 0)) {
        //     tv1?.text = pl[p0].packingLine.ItemNo.toString()
        //     tv2?.text = pl[p0].packingLine.CrossReferenceNo.toString()
        //     tv3?.text = pl[p0].TotalQuantity.toString()
        //     q1 = pl[p0].TotalQuantity
        //     var counter1 = 0
        //     for (m in pi) {
        //         if ((pl[p0].packingLine.ItemNo == m.prescanInnerCarton.CSPN) &&
        //                 (pl[p0].packingLine.CrossReferenceNo == m.prescanInnerCarton.CrossReferenceNo)) {
        //             tv4?.text = m.TotalQuantity.toString()
        //             q2 = m.TotalQuantity
        //             pi2.drop(counter1)
        //         }
        //         counter1 = counter1 + 1
        //     }
        //     if (q1 != q2)
        //     {
        //         linearLayout1.setBackgroundColor(Color.RED)
        //     }
        // }
        // else
        // {
        //     tv1?.text = pi2[p0-pl.size].prescanInnerCarton.CSPN.toString()
        //     tv2?.text = pi2[p0-pl.size].prescanInnerCarton.CrossReferenceNo.toString()
        //     tv4?.text = pi2[p0-pl.size].TotalQuantity.toString()
        //     linearLayout1.setBackgroundColor(Color.RED)
        // }

    }
}