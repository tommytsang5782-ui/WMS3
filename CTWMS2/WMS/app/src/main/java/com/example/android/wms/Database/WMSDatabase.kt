package com.example.android.wms.Database

import android.content.Context
import android.util.Log
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.room.TypeConverters
import androidx.sqlite.db.SupportSQLiteDatabase
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.time.LocalDateTime
import java.time.ZoneId
import java.util.*

//Database INSTANCE and Insert initialization data
@Database(entities = [PackingHeader::class,PackingLine::class,ScanLabelString::class,User::class,Prescan::class,Mapping::class,
    OuterCarton::class,InnerCarton::class,
    ReadyToSend::class,PrescanOuterCarton::class,PrescanInnerCarton::class,PackingMapping::class,
    Item::class,CustomerGroup::class,Printer::class], version = 29, exportSchema = false)
@TypeConverters(Converters::class)
abstract class WMSDatabase: RoomDatabase() {
    abstract fun Dao() : WMSDao
    companion object {
        @Volatile
        private var INSTANCE: WMSDatabase? = null
        private var G_context: Context? = null


        fun getInstance(
            context: Context,
            scope: CoroutineScope,
        ): WMSDatabase {
            // if the INSTANCE is not null, then return it,
            // if it is, then create the database
            G_context = context
            return INSTANCE ?: synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    WMSDatabase::class.java,
                    "WMSDatabase"
                )
                    // Wipes and rebuilds instead of migrating if no Migration object.
                    // Migration is not part of this codelab.
                    .fallbackToDestructiveMigration()
                    .addCallback(WMSDatabaseCallback(scope))
                    .allowMainThreadQueries()
                    .addMigrations()
                    .setJournalMode(RoomDatabase.JournalMode.TRUNCATE)
                    .build()

                INSTANCE = instance
                // return instance
                instance
            }
        }

        private class WMSDatabaseCallback(
            private val scope: CoroutineScope,
        ) : Callback() {
            /**
             * Override the onCreate method to populate the database.
             */
            override fun onCreate(db: SupportSQLiteDatabase) {
                super.onCreate(db)
                // If you want to keep the data through app restarts,
                // comment out the following line.
                INSTANCE?.let { database ->
                    scope.launch(Dispatchers.IO) {
                        populateDatabase(database.Dao())
                    }
                }
            }
        }

        /**
         * Populate the database in a new coroutine.
         * If you want to start with more words, just add them.
         */
        suspend fun populateDatabase(dao: WMSDao) {
            dao.GetUserList()
            var currentDateTime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
            var scanLabelString = ScanLabelString(1,"","",0,false,"",currentDateTime,"",currentDateTime,"",false)
            dao.insertLabelData(scanLabelString)
            dao.deleteScanLabelString(1)
            var customerGroup = CustomerGroup("Reallytek","Reallytek","OuterIncludeInner",false,"","")
            dao.insertCustomerGroup(customerGroup)
        }
    }
}