package com.example.android.wms.common

object UserManager {
    private var _userID: String = ""

    val UserID: String get() = _userID
    /**
     * 登录：仅保存到内存
     */
    fun login(userId: String) {
        if (userId.isBlank()) {
            throw IllegalArgumentException("User ID 不能為空")
        }
        _userID = userId
    }

    /**
     * 登出：清空内存
     */
    fun logout() {
        _userID = ""
    }
}