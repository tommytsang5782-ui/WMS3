package com.example.android.wms.Setting

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Printer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class SettingViewModel  (private val repository: SettingRepository, application: Application): ViewModel(),
        CoroutineScope by MainScope() {
    suspend fun  selectCustomerGroupList(): List<CustomerGroup> {
        return repository.selectCustomerGroupList()
    }

    suspend fun  selectCustomerGroup(code: String): CustomerGroup {
        return repository.selectCustomerGroup(code)
    }

    suspend fun  selectPrinterList(): List<Printer> {
        return repository.selectPrinterList()
    }

    suspend fun  selectPrinter(code: String): Printer {
        return repository.selectPrinter(code)
    }

    suspend fun  updatePrinter(item: Printer) {
        repository.updatePrinter(item)
    }

    suspend fun  updateCustomerGroup(item: CustomerGroup) {
        repository.updateCustomerGroup(item)
    }
    suspend fun  deleteAllLabelData()
    {
        repository.deleteAllLabelData()
    }
    suspend fun  deleteAllOuterCarton()
    {
        repository.deleteAllOuterCarton()
    }
    suspend fun  deleteAllInnerCarton(){
        repository.deleteAllInnerCarton()
    }
    suspend fun  selectDefaultPrinter(): Printer{
        return repository.selectDefaultPrinter()
    }
}


class SettingViewModelFactory(private val repository: SettingRepository,
                              private val application: Application
): ViewModelProvider.Factory{
    override fun  <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(SettingViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return SettingViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}