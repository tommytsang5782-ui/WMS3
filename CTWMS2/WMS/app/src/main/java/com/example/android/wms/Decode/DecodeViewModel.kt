package com.example.android.wms.Decode

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewmodel.CreationExtras
import com.example.android.wms.Database.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class DecodeViewModel
    (private val repository: DecodeRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {

    suspend fun SelectPrescanInnerCarton_OuterCarton(DocumentNo:String,OuterCartonLineNo:Int):List<PrescanInnerCarton>{
        return repository.SelectPrescanInnerCarton_OuterCarton(DocumentNo,OuterCartonLineNo)
    }
    suspend fun FindPrescanOuterList(prescanNo: String):Boolean{
        return repository.FindPrescanOuterList(prescanNo)
    }
    suspend fun FindCarton(DocumentNo:String,CartonID:String): Boolean{
        return repository.FindCarton(DocumentNo,CartonID)
    }
    suspend fun SelectPackingMapping(Packingo: String) : PackingMapping {
        return repository.SelectPackingMapping(Packingo)
    }
    suspend fun SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton> {
        return repository.SelectPrescanOuterCarton_DocNo(DocumentNo)
    }
    suspend fun getMapping_ScanItemNo(itemNo:String): Mapping {
        return repository.getMapping_ScanItemNo(itemNo)
    }
    suspend fun GetPL(PackingNo: String): List<PackingLine>? {
        return repository.GetPL(PackingNo)
    }
    suspend fun NumberOfScanned(PackingNo:String,PackingLineNo:Int):Int{
        return repository.NumberOfScanned(PackingNo,PackingLineNo)
    }
    suspend fun updatePL(item: PackingLine) {
        return repository.updatePL(item)
    }

}



class DecodeViewModelFactory(private val repository: DecodeRepository,
                             private val application: Application
): ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>, extras: CreationExtras): T {
        if (modelClass.isAssignableFrom(DecodeViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return DecodeViewModel(repository, application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class: ${modelClass.name}")
    }
}