package com.example.android.wms.Welcome

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.User
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class WelcomeViewModel(private val repository: WelcomeRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {
    suspend fun initDatabase(){
        repository.GetUserList()
    }
}

class WelcomeViewModelFactory(private val repository: WelcomeRepository,
                           private val application: Application
): ViewModelProvider.Factory{
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(WelcomeViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return WelcomeViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}