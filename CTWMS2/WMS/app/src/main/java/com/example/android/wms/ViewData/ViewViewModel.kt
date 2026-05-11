package com.example.android.wms.ViewData

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class ViewViewModel(private val repository: ViewRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {
    suspend fun  GetUserList():List<User> {
        return repository.GetUserList()
    }
    suspend fun  GetPHNoList():List<String> {
        return repository.GetPHNoList()
    }
    suspend fun  GetOpenPHNoList(): List<String> {
        return repository.GetOpenPHNoList()
    }
    suspend fun  GetFinishPHNoList(): List<String> {
        return repository.GetFinishPHNoList()
    }

    suspend fun  GetPLLineNoList(PackingNo: String): List<Int> {
        return repository.GetPLLineNoList(PackingNo)
    }
    suspend fun  GetPL(PackingNo: String): List<PackingLine> {
        return repository.GetPL(PackingNo)
    }
    suspend fun  GetPL3(PackingNo: String): List<PackingLine1> {
        return repository.GetPL3(PackingNo)
    }
    suspend fun  getPHbyNo(PackingNo: String): PackingHeader? {
        return repository.getPHbyNo(PackingNo)
    }
    suspend fun  GetLabelData(PackingNo: String): List<ScanLabelString> {
        return repository.GetLabelData(PackingNo)
    }
    suspend fun  GetPrescanNoList(): List<String> {
        return repository.GetPrescanNoList()
    }
    suspend fun  GetPrescanList(): List<Prescan> {
        return repository.GetPrescanList()
    }
    suspend fun  selectPrescanInnerCarton_OuterCarton(DocumentNo:String ,OuterCartonLineNo:Int): List<PrescanInnerCarton> {
        return repository.selectPrescanInnerCarton_OuterCarton(DocumentNo,OuterCartonLineNo)
    }
    suspend fun  SelectPrescanInnerCarton3(DocumentNo:String ): List<PrescanInnerCarton1> {
        return repository.SelectPrescanInnerCarton3(DocumentNo)
    }
    suspend fun  getPrescanByPackingNoAndLineNo(PackingNo:String,PackingLineNo:Int): List<OuterCarton> {
        return repository.getPrescanByPackingNoAndLineNo(PackingNo,PackingLineNo)
    }
    suspend fun  getMappingList(): List<Mapping> {
        return repository.getMappingList()
    }
    suspend fun  SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton> {
        return repository.SelectPrescanOuterCarton_DocNo(DocumentNo)
    }
    suspend fun  selectPrinterList(): List<Printer>
    {
        return repository.selectPrinterList()
    }
    suspend fun  selectCustomerGroupList(): List<CustomerGroup>{
        return repository.selectCustomerGroupList()
    }
    suspend fun  selectItemList(): List<Item>{
        return repository.selectItemList()
    }
}

class ViewViewModelFactory(private val repository: ViewRepository,
                           private val application: Application
): ViewModelProvider.Factory{
    override fun  <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(ViewViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return ViewViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}