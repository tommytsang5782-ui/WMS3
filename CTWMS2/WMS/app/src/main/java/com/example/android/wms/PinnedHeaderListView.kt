package com.example.android.wms

import android.content.Context
import android.graphics.Canvas
import android.opengl.ETC1.getWidth
import android.util.AttributeSet
import android.view.View
import android.view.View.MeasureSpec
import android.view.ViewGroup
import android.widget.*

class PinnedHeaderListView  {
    /*
    private var mOnScrollListener: OnScrollListener? = null

    interface PinnedSectionedHeaderAdapter {
        fun isSectionHeader(position: Int): Boolean
        fun getSectionForPosition(position: Int): Int
        fun getSectionHeaderView(section: Int, convertView: View?, parent: ViewGroup?): View
        fun getSectionHeaderViewType(section: Int): Int
        val count: Int
    }

    private var mAdapter: PinnedSectionedHeaderAdapter? = null
    private var mCurrentHeader: View? = null
    private var mCurrentHeaderViewType = 0
    private var mHeaderOffset = 0f
    private var mShouldPin = true
    private var mCurrentSection = 0
    private var mWidthMode = 0
    private var mHeightMode = 0

    constructor(context: Context?) : super(context) {
        super.setOnScrollListener(this)
    }

    constructor(context: Context?, attrs: AttributeSet?) : super(context, attrs) {
        super.setOnScrollListener(this)
    }

    constructor(context: Context?, attrs: AttributeSet?, defStyle: Int) : super(context, attrs, defStyle) {
        super.setOnScrollListener(this)
    }

    fun setPinHeaders(shouldPin: Boolean) {
        mShouldPin = shouldPin
    }

    override fun setAdapter(adapter: ListAdapter?) {
        mCurrentHeader = null
        mAdapter = adapter
        super.setAdapter(adapter)
    }

    override fun onScroll(view: AbsListView?, firstVisibleItem: Int, visibleItemCount: Int, totalItemCount: Int) {
        var firstVisibleItem = firstVisibleItem
        if (mOnScrollListener != null) {
            mOnScrollListener.onScroll(view, firstVisibleItem, visibleItemCount, totalItemCount)
        }
        if (mAdapter == null || mAdapter!!.count == 0 || !mShouldPin || firstVisibleItem < getHeaderViewsCount()) {
            mCurrentHeader = null
            mHeaderOffset = 0.0f
            for (i in firstVisibleItem until firstVisibleItem + visibleItemCount) {
                val header: View = getChildAt(i)
                if (header != null) {
                    header.setVisibility(VISIBLE)
                }
            }
            return
        }
        firstVisibleItem -= getHeaderViewsCount()
        val section = mAdapter!!.getSectionForPosition(firstVisibleItem)
        val viewType = mAdapter!!.getSectionHeaderViewType(section)
        mCurrentHeader = getSectionHeaderView(section, if (mCurrentHeaderViewType != viewType) null else mCurrentHeader)
        ensurePinnedHeaderLayout(mCurrentHeader)
        mCurrentHeaderViewType = viewType
        mHeaderOffset = 0.0f
        for (i in firstVisibleItem until firstVisibleItem + visibleItemCount) {
            if (mAdapter!!.isSectionHeader(i)) {
                val header: View = getChildAt(i - firstVisibleItem)
                val headerTop: Float = header.getTop()
                val pinnedHeaderHeight: Float = mCurrentHeader.getMeasuredHeight()
                header.setVisibility(VISIBLE)
                if (pinnedHeaderHeight >= headerTop && headerTop > 0) {
                    mHeaderOffset = headerTop - header.getHeight()
                } else if (headerTop <= 0) {
                    header.setVisibility(INVISIBLE)
                }
            }
        }
        invalidate()
    }

    override fun onScrollStateChanged(view: AbsListView?, scrollState: Int) {
        if (mOnScrollListener != null) {
            mOnScrollListener.onScrollStateChanged(view, scrollState)
        }
    }

    private fun getSectionHeaderView(section: Int, oldView: View?): View {
        val shouldLayout = section != mCurrentSection || oldView == null
        val view: View = mAdapter!!.getSectionHeaderView(section, oldView, this)
        if (shouldLayout) {
            // a new section, thus a new header. We should lay it out again
            ensurePinnedHeaderLayout(view)
            mCurrentSection = section
        }
        return view
    }

    private fun ensurePinnedHeaderLayout(header: View?) {
        if (header.isLayoutRequested()) {
            val widthSpec = MeasureSpec.makeMeasureSpec(getMeasuredWidth(), mWidthMode)
            val heightSpec: Int
            val layoutParams: ViewGroup.LayoutParams = header.getLayoutParams()
            heightSpec = if (layoutParams != null && layoutParams.height > 0) {
                MeasureSpec.makeMeasureSpec(layoutParams.height, MeasureSpec.EXACTLY)
            } else {
                MeasureSpec.makeMeasureSpec(0, MeasureSpec.UNSPECIFIED)
            }
            header.measure(widthSpec, heightSpec)
            header.layout(0, 0, header.getMeasuredWidth(), header.getMeasuredHeight())
        }
    }

    protected override fun dispatchDraw(canvas: Canvas) {
        super.dispatchDraw(canvas)
        if (mAdapter == null || !mShouldPin || mCurrentHeader == null) return
        val saveCount: Int = canvas.save()
        canvas.translate(0, mHeaderOffset)
        canvas.clipRect(0, 0, getWidth(), mCurrentHeader.getMeasuredHeight()) // needed
        // for
        // <
        // HONEYCOMB
        mCurrentHeader.draw(canvas)
        canvas.restoreToCount(saveCount)
    }

    override fun setOnScrollListener(l: OnScrollListener?) {
        mOnScrollListener = l
    }

    protected override fun onMeasure(widthMeasureSpec: Int, heightMeasureSpec: Int) {
        super.onMeasure(widthMeasureSpec, heightMeasureSpec)
        mWidthMode = MeasureSpec.getMode(widthMeasureSpec)
        mHeightMode = MeasureSpec.getMode(heightMeasureSpec)
    }

    fun setOnItemClickListener(listener: OnItemClickListener?) {
        super.setOnItemClickListener(listener)
    }

    abstract class OnItemClickListener : OnItemClickListener() {
        fun onItemClick(adapterView: AdapterView<*>, view: View?, rawPosition: Int, id: Long) {
            val adapter: SectionedBaseAdapter
            adapter = if (adapterView.adapter.javaClass == HeaderViewListAdapter::class.java) {
                val wrapperAdapter = adapterView.adapter as HeaderViewListAdapter
                wrapperAdapter.wrappedAdapter as SectionedBaseAdapter
            } else {
                adapterView.adapter as SectionedBaseAdapter
            }
            val section: Int = adapter.getSectionForPosition(rawPosition)
            val position: Int = adapter.getPositionInSectionForPosition(rawPosition)
            if (position == -1) {
                onSectionClick(adapterView, view, section, id)
            } else {
                onItemClick(adapterView, view, section, position, id)
            }
        }

        abstract fun onItemClick(adapterView: AdapterView<*>?, view: View?, section: Int, position: Int, id: Long)
        abstract fun onSectionClick(adapterView: AdapterView<*>?, view: View?, section: Int, id: Long)
    }

     */
}