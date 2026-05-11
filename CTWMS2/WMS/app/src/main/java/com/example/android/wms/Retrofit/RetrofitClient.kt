package com.example.android.wms.Retrofit

import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.*
import com.example.android.wms.Database.Mapping
import com.example.android.wms.Database.User
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object RetrofitClient {

    private val retrofitInstances = mutableMapOf<String, Retrofit>()

    internal fun getRetrofitInstance(baseUrl: String): Retrofit {
        return retrofitInstances.getOrPut(baseUrl) {
            Retrofit.Builder()
                    .baseUrl(baseUrl)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build()
        }
    }

    @PublishedApi
    internal fun <T> createServiceInternal(baseUrl: String, service: Class<T>): T {
        val retrofit = getRetrofitInstance(baseUrl)
        return retrofit.create(service)
    }

    inline fun <reified T> createService(baseUrl: String): GenericApiService<T> {
        return createServiceInternal(baseUrl, GenericApiService::class.java) as GenericApiService<T>
    }

    //// 定义 API 服务实例
    //val itemApiService: GenericApiService<Item> by lazy {
    //    createService("${Constants.BASE_API_URL}items/")
    //}
//
    //val userApiService: GenericApiService<User> by lazy {
    //    createService("${Constants.BASE_API_URL}users/")
    //}
//
    //val mappingApiService: GenericApiService<Mapping> by lazy {
    //    createService("${Constants.BASE_API_URL}mappings/")
    //}
    //// 定义 PackingHeader API 服务实例
    //val packingHeaderApiService: GenericApiService<PackingHeader> by lazy {
    //    createService("${Constants.BASE_API_URL}packingheaders/")
    //}
}
