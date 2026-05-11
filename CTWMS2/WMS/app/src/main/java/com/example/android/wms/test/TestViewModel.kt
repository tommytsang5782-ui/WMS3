package com.example.android.wms.test

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class TestViewModel(private val repository: TestRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {
    suspend fun initDatabase(){
        repository.GetUserList()
    }
}

class TestViewModelFactory(private val repository: TestRepository,
                              private val application: Application
): ViewModelProvider.Factory{
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(TestViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return TestViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}