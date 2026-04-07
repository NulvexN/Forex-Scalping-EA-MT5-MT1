// //////////////////////////////////////////////////////////////////////////
// // ��չ�Ի���
// // 1 ֧�ֹ�����ʾ
// // author: WT
// // date:   2013-07-04
// //////////////////////////////////////////////////////////////////////////

// #if !defined(AFX_DIALOG_H__20389200_7D5B_42F5_97C0_248E2F2BA142__INCLUDED_)
// #define AFX_DIALOG_H__20389200_7D5B_42F5_97C0_248E2F2BA142__INCLUDED_

// #if _MSC_VER > 1000
// #pragma once
// #endif // _MSC_VER > 1000
// // Dialog.h : header file
// #include "Label.h"

// /////////////////////////////////////////////////////////////////////////////
// // Dialog dialog
// // �Զ���Ի���֧�ֹ�����ʾ���˵�״̬����
// class Dialog : public CDialog
// {
// // Construction
// public:
//     Dialog( UINT nIDTemplate, CWnd* pParentWnd = NULL );   // standard constructor

// // Dialog Data
//     //{{AFX_DATA(Dialog)
//         // NOTE: the ClassWizard will add data members here
//     //}}AFX_DATA

//     BOOL Create( CWnd * pParentWnd )
//     {
//         return CDialog::Create( m_nIDTemplate, pParentWnd );
//     }

// // Overrides
//     // ClassWizard generated virtual function overrides
// //{{AFX_VIRTUAL(Dialog)
// public:
//     virtual BOOL PreTranslateMessage(MSG* pMsg);
// protected:
//     virtual BOOL OnInitDialog();
// //}}AFX_VIRTUAL

// // Implementation
// protected:
//     UINT m_nIDTemplate; // �Ի���ģ����ԴID
//     CToolTipCtrl m_ToolTips; // ������ʾ�ؼ�
//     //std::vector< winplus::SimplePointer<Label> > m_allLabels; // ��ǩ���������໯Static���Ի�

//     // Generated message map functions
//     //{{AFX_MSG(Dialog)
//     afx_msg void OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct);
//     afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
//     //}}AFX_MSG
//     afx_msg BOOL OnEraseBkgnd( CDC * pDC );

//     // �������˵�ʱ���ã����²˵����״̬
//     afx_msg void OnInitMenuPopup( CMenu * pMenu, UINT nIndex, BOOL bSysMenu );

//     // ��������ʾ��Ҫ��ʾ�ı�ʱ����
//     afx_msg BOOL OnToolTipText( UINT id, NMHDR * pNMHDR, LRESULT * pResult );

//     DECLARE_MESSAGE_MAP()
// };

// //{{AFX_INSERT_LOCATION}}
// // Microsoft Visual C++ will insert additional declarations immediately before the previous line.

// #endif // !defined(AFX_DIALOG_H__20389200_7D5B_42F5_97C0_248E2F2BA142__INCLUDED_)
