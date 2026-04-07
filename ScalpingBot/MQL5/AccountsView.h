// //////////////////////////////////////////////////////////////////////////
// // �˻��б���ͼ
// // author: WT
// //////////////////////////////////////////////////////////////////////////

// #if !defined(AFX_CHILDVIEW_H__7450EEF0_270E_450D_B442_628F9161B81A__INCLUDED_)
// #define AFX_CHILDVIEW_H__7450EEF0_270E_450D_B442_628F9161B81A__INCLUDED_

// #if _MSC_VER > 1000
// #pragma once
// #endif // _MSC_VER > 1000

// /////////////////////////////////////////////////////////////////////////////
// // AccountsView window

// class AccountsView : public CListView, public IUpdateListView
// {
// // Construction
// public:
//     AccountsView();
//     virtual ~AccountsView();

// // Attributes
// public:
// // Operations
//     /** \brief Method UpdateList
//      *
//      *  \param flag ����UPDATE_LOAD_DATA or UPDATE_LIST_ITEMS
//      *  \param itemIndex ������-1��ʾ����ȫ�� */
//     virtual void UpdateList( int flag = UPDATE_LOAD_DATA | UPDATE_LIST_ITEMS, long itemIndex = -1 );
// // Overrides
//     // ClassWizard generated virtual function overrides
//     //{{AFX_VIRTUAL(AccountsView)
// protected:
//     virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
//     //}}AFX_VIRTUAL

// // Implementation
// public:

// protected:
//     // ͼ���б�
//     CImageList m_iconList;
//     // ����洢�˻���¼
//     AccountArray m_accounts;

// // Generated message map functions
//     //{{AFX_MSG(AccountsView)
//     afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
//     afx_msg void OnSize(UINT nType, int cx, int cy);
//     afx_msg void OnListViewRClick(NMHDR* pNMHDR, LRESULT* pResult);
//     afx_msg void OnListViewActivated(NMHDR* pNMHDR, LRESULT* pResult);
//     afx_msg void OnListStyleIcon();
//     afx_msg void OnListStyleReport();
//     //}}AFX_MSG
//     afx_msg void OnUpdateListStyle(CCmdUI* pCmdUI);
//     DECLARE_MESSAGE_MAP()

//     friend class MainFrame;
// };

// /////////////////////////////////////////////////////////////////////////////

// //{{AFX_INSERT_LOCATION}}
// // Microsoft Visual C++ will insert additional declarations immediately before the previous line.

// #endif // !defined(AFX_CHILDVIEW_H__7450EEF0_270E_450D_B442_628F9161B81A__INCLUDED_)
