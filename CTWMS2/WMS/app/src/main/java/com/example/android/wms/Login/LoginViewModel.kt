package com.example.android.wms.Login

import android.app.Application
import android.text.Editable
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.User
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.MainScope

class LoginViewModel(private val repository: LoginRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {

    //private var binding: ActivityLoginBinding = Unit

    var Username:String? = null
    var Password:String? = null

    private val viewModelJob = Job()
    private val uiScope = CoroutineScope(Dispatchers.Main + viewModelJob)
    var _navigatetoUserDetails: Boolean = false

    var _errorToast: Boolean = false

    var _errorToastUsername: Boolean = false
    var _errorToastInvalidPassword: Boolean = false

    suspend fun loginButton(inputUsername: Editable, inputPassword: Editable) {
        _errorToast = false
        _navigatetoUserDetails = false
        _errorToastInvalidPassword = false
        _errorToastUsername = false
        Username = inputUsername.toString().trim()
        Password = inputPassword.toString()
        if (Username.isNullOrEmpty() || Password.isNullOrEmpty()) {
            _errorToast = true
        } else {
            val usersNames = repository.getUserName(Username.orEmpty())
            if (usersNames != null) {
                if(usersNames.Password == Password){
                    _navigatetoUserDetails = true
                }else{
                    _errorToastInvalidPassword = true
                }
            } else {
                _errorToastUsername = true
            }
        }
    }
    suspend fun GetUserList():List<User> {
        return repository.GetUserList()
    }
    override fun onCleared() {
        super.onCleared()
        viewModelJob.cancel()
    }
}

class LoginViewModelFactory(private val repository: LoginRepository,
                            private val application: Application
): ViewModelProvider.Factory{
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(LoginViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return LoginViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}