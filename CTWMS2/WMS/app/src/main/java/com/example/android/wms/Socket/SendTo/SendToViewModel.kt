package com.example.android.wms.Socket.SendTo

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.ReadyToSend
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class SendToViewModel(private val repository: SendToRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {
    suspend fun insertReadyToSend(readyToSend: ReadyToSend){
        repository.insertReadyToSend(readyToSend)
    }
}



class SendToViewModelFactory(private val repository: SendToRepository,
                             private val application: Application
): ViewModelProvider.Factory{
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(SendToViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return SendToViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}