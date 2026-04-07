
// #include "Cassette.h"
// #include "CassetteApp.h"
// #include "AccountEditingDlg.h"

// #ifdef _DEBUG
// #define new DEBUG_NEW
// #undef THIS_FILE
// static char THIS_FILE[] = __FILE__;
// #endif

// /////////////////////////////////////////////////////////////////////////////
// // AccountEditingDlg dialog
// AccountEditingDlg::AccountEditingDlg( CWnd * parent, bool isAdd, winplus::Mixed * accountFields ) :
//     Dialog( AccountEditingDlg::IDD, GetDesktopWindow() ),
//     m_pWndParent(parent),
//     m_isAdd(isAdd),
//     m_accountFields(accountFields)
// {
//     //{{AFX_DATA_INIT(AccountEditingDlg)
//     //}}AFX_DATA_INIT
// }

// void AccountEditingDlg::DoDataExchange( CDataExchange * pDX )
// {
//     Dialog::DoDataExchange(pDX);
//     //{{AFX_DATA_MAP(AccountEditingDlg)

//     //}}AFX_DATA_MAP

//     DDX_CBIndex(pDX, IDC_COMBO_CATES, m_cateIndex);

//     Account account;
//     account = *m_accountFields;

//     DDX_Text(pDX, IDC_EDIT_MYNAME, account.m_myName);
//     DDX_Text(pDX, IDC_EDIT_ACCOUNTNAME, account.m_accountName);
//     DDX_Text(pDX, IDC_EDIT_ACCOUNTPWD, account.m_accountPwd);
//     DDX_Text(pDX, IDC_EDIT_SAFERANK, account.m_safeRank);
//     DDX_Text(pDX, IDC_EDIT_COMMENT, account.m_comment);

//     account.assignTo( m_accountFields, "myname,account_name,account_pwd,safe_rank,comment" );
// }

// BEGIN_MESSAGE_MAP(AccountEditingDlg, Dialog)
//     //{{AFX_MSG_MAP(AccountEditingDlg)
//     ON_CBN_SELCHANGE(IDC_COMBO_CATES, OnSelChangeComboCates)
//     //}}AFX_MSG_MAP
//     ON_WM_SHOWWINDOW()
// END_MESSAGE_MAP()

// int AccountEditingDlg::GetSafeRankByCateId( int cateId ) const
// {
//     int count = (int)m_cateIds2.GetSize();
//     int i;
//     for ( i = 0; i < count; ++i )
//     {
//         if ( cateId == m_cateIds2[i] )
//         {
//             return m_typeSafeRanks[i];
//         }
//     }
//     return 0;
// }

// /////////////////////////////////////////////////////////////////////////////
// // AccountEditingDlg message handlers

// BOOL AccountEditingDlg::OnInitDialog() 
// {
//     Dialog::OnInitDialog();
//     m_ToolTips.SetTipTextColor( RGB( 255, 96, 0 ) ); // ������ʾ�ı���ɫ

//     // ���ñ���
//     SetWindowText( m_isAdd ? _T("�����˻�...") : _T("�޸��˻�...") );

//     // ����������Ϣ
//     int catesCount = LoadAccountCates( g_theApp.GetDatabase(), &m_cates );
//     LoadAccountCatesSafeRank( g_theApp.GetDatabase(), &m_cateIds2, &m_typeSafeRanks );
//     // �ڲ���ѡ�����ͨ��������ȷ��ѡ����ģ�����ⲿ���ݵ�Cate ID��Ҫת��Ϊ��������������ѡ����Ӧ��Cate
//     m_cateIndex = -1;
//     int i;
//     // ID to Index
//     for ( i = 0; i < catesCount; ++i )
//     {
//         if ( m_cates[i].m_id == (int)(*m_accountFields)["cate"] )
//         {
//             m_cateIndex = i;
//             break;
//         }
//     }
//     // Add data to combobox
//     CComboBox * pCboCates = (CComboBox *)GetDlgItem(IDC_COMBO_CATES);
//     for ( i = 0; i < catesCount; ++i )
//     {
//         pCboCates->AddString(m_cates[i].m_cateName);
//     }

//     UpdateData(FALSE);

//     // ��������Ӳ�ѡ�������࣬�򴥷���Ͽ�ѡ��ı��¼�
//     if ( m_isAdd && m_cateIndex != -1 )
//         SendMessage( WM_COMMAND, MAKEWPARAM( IDC_COMBO_CATES, CBN_SELCHANGE ), (LPARAM)pCboCates->GetSafeHwnd() );

//     // ����
//     CRect rc1, rc2;
//     CPoint pt;
//     if ( Window_CalcCenter( *this, *m_pWndParent, false, &rc1, &rc2, &pt ) )
//         Window_Center( *this, *m_pWndParent );
//     else
//         Window_Center( *this, NULL );

//     this->SetForegroundWindow();

//     return TRUE;  // return TRUE unless you set the focus to a control
//                   // EXCEPTION: OCX Property Pages should return FALSE
// }

// void AccountEditingDlg::OnOK() 
// {
//     UpdateData(TRUE);

//     if ( (*m_accountFields)["myname"].refAnsi().empty() )
//     {
//         WarningError( _T("���Ʋ���Ϊ��"), _T("����") );
//         return;
//     }

//     if ( m_cateIndex == -1 )
//     {
//         WarningError( _T("����ѡ��һ���˻�����"), _T("����") );
//         return;
//     }
//     // Index to Id
//     (*m_accountFields)["cate"] = m_cates[m_cateIndex].m_id;

//     EndDialog(IDOK);
// }

// void AccountEditingDlg::OnSelChangeComboCates() 
// {
//     if ( !m_isAdd ) return; // ֻ�������˻�ʱ��Ҫ�˹���
//     UpdateData(TRUE);
//     CComboBox * pCboCates = (CComboBox *)GetDlgItem(IDC_COMBO_CATES);
//     (*m_accountFields)["safe_rank"] = GetSafeRankByCateId(m_cates[m_cateIndex].m_id);
//     (*m_accountFields)["comment"] = (LPCTSTR)m_cates[m_cateIndex].m_cateDesc;
//     CString tmpMyName;
//     pCboCates->GetLBText( m_cateIndex, tmpMyName );
//     tmpMyName = _T("�ҵ�") + tmpMyName + _T("�˻�");
//     (*m_accountFields)["myname"] = (LPCTSTR)GetCorrectAccountMyName( g_theApp.GetDatabase(), g_theApp.m_loginedUser.m_id, tmpMyName );
//     UpdateData(FALSE);
// }


// void AccountEditingDlg::OnShowWindow( BOOL bShow, UINT nStatus )
// {
//     // �ö�
//     this->SetWindowPos( &wndTopMost, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE );
// }
