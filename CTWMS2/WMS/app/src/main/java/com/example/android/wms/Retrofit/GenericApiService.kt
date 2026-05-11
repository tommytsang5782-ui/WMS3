package com.example.android.wms.Retrofit

import retrofit2.http.*

interface GenericApiService<T> {

    @GET(".")
    suspend fun getAll(): List<T>

    @GET("{id}")
    suspend fun getById(@Path("id") id: String): T

    @POST(".")
    suspend fun create(@Body entity: T): T

    @PUT("{id}")
    suspend fun update(@Path("id") id: String, @Body entity: T): T

    @DELETE("{id}")
    suspend fun delete(@Path("id") id: String)
}
