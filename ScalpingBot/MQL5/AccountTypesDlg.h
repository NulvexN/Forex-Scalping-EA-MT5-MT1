// #if !defined(AFX_ACCOUNTTYPESDLG_H__BFB9B0BC_90D5_4205_98DE_84C757C2713A__INCLUDED_)
// #define AFX_ACCOUNTTYPESDLG_H__BFB9B0BC_90D5_4205_98DE_84C757C2713A__INCLUDED_

// #if _MSC_VER > 1000
// #pragma once
// #endif // _MSC_VER > 1000
// // AccountTypesDlg.h : header file
// #include "Dialog.h"

// /////////////////////////////////////////////////////////////////////////////
// // AccountTypesDlg dialog

// class AccountTypesDlg : public Dialog, public IUpdateListView
// {
// // Construction
// public:
//     AccountTypesDlg( CWnd * parent = NULL );   // standard constructor

// // Dialog Data
//     //{{AFX_DATA(AccountTypesDlg)
//     enum { IDD = IDD_ACCOUNT_TYPES };
//         // NOTE: the ClassWizard will add data members here
//     //}}AFX_DATA

//     virtual void UpdateList( int flag = UPDATE_LOAD_DATA | UPDATE_LIST_ITEMS, long itemIndex = -1 );

// // Overrides
//     // ClassWizard generated virtual function overrides
//     //{{AFX_VIRTUAL(AccountTypesDlg)
//     protected:
//     virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
//     //}}AFX_VIRTUAL

// // Implementation
// protected:
//     AccountTypeArray m_types;

//     // Generated message map functions
//     //{{AFX_MSG(AccountTypesDlg)
//     virtual BOOL OnInitDialog();
//     afx_msg void OnListActivated(NMHDR* pNMHDR, LRESULT* pResult);
//     afx_msg void OnListRClick(NMHDR* pNMHDR, LRESULT* pResult);
//     afx_msg void OnAdd();
//     afx_msg void OnModify();
//     afx_msg void OnDelete();
//     //}}AFX_MSG
//     afx_msg void OnUpdateModifyDeleteMenu( CCmdUI * pCmdUI );
//     DECLARE_MESSAGE_MAP()
// };

// //{{AFX_INSERT_LOCATION}}
// // Microsoft Visual C++ will insert additional declarations immediately before the previous line.

// #endif // !defined(AFX_ACCOUNTTYPESDLG_H__BFB9B0BC_90D5_4205_98DE_84C757C2713A__INCLUDED_)
