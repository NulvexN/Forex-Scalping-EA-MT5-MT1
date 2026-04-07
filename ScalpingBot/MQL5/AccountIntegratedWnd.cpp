
// #include "Cassette.h"
// #include "CassetteApp.h"
// #include "MainFrame.h"
// #include "AccountIntegratedWnd.h"
// #include "AccountEditingDlg.h"

// #include <playsoundapi.h>

// // AccountIntegratedWnd -------------------------------------------------------------------
// AccountIntegratedWnd::AccountIntegratedWnd( CWnd * pParentWnd, LPCTSTR lpszWindowName, const RECT& rect ) :
//     m_primaryFont( L"����", 24, 0, UnitPixel ),
//     m_penBlack( Color( 0, 0, 0 ) ),
//     m_penHalfWhite( Color( 128, 255, 255, 255 ), 0.5f ),
//     m_penWhite( Color( 255, 255, 255 ) ),
//     m_brushHalfWhite( Color( 64, 255, 255, 255 ) ),
//     m_brushBlack( Color( 0, 0, 0 ) ),
//     m_brushWhite( Color( 255, 255, 255 ) )
// {
//     m_ptCurMouse = CPoint( -100, -100 );
//     m_radiusMouseCircle = 8;
//     m_sfHVCenter.SetAlignment(StringAlignmentCenter);
//     m_sfHVCenter.SetLineAlignment(StringAlignmentCenter);

//     // ��������
//     CWnd::CreateEx( 0, AfxRegisterWndClass( CS_HREDRAW | CS_VREDRAW ), lpszWindowName, WS_POPUP | WS_CAPTION | WS_SYSMENU, rect, pParentWnd, 0, NULL );

// }

// AccountIntegratedWnd::~AccountIntegratedWnd()
// {
// }

// void AccountIntegratedWnd::SetAccountsInfo( AccountCate const & cate, AccountArray const & accounts )
// {
//     m_accounts.RemoveAll();

//     m_cate = cate;
//     int nAccountsCount = accounts.GetCount();

//     // �߾�
//     constexpr int padding = 4;
//     // ���
//     constexpr int itemHeight = 48;
//     int height = itemHeight;
//     for ( int i = 0; i < nAccountsCount; i++, height += itemHeight )
//     {
//         AccountContext ctx;
//         ctx.account = accounts[i];

//         ctx.accountRect = RectF( m_contentRect.X + padding, m_contentRect.Y + height - (itemHeight - padding), m_contentRect.Width - 1 - padding - padding, (itemHeight - padding) );
        
//         RectF targetRect = ctx.accountRect;
//         targetRect.Height /= 2;
//         targetRect.Inflate( -padding, 0 );
//         ctx.accountField0Rect = targetRect;

//         targetRect.Offset( 0, targetRect.Height );
//         ctx.accountField1Rect = targetRect;

//         m_accounts.Add(ctx);
//     }
// }

// void AccountIntegratedWnd::RefreshAllCreate()
// {
//     // ������ض���
//     GetClientRect(&m_rcClient);
//     // ��������
//     m_contentRect = RectF( 10, 40, m_rcClient.Width() - 20, m_rcClient.Height() - 40 - 10 );

//     // ���뱳��ͼ
//     m_loadedBgImg.attachNew( new Bitmap( AfxGetApp()->m_hInstance, MAKEINTRESOURCEW(IDB_INTEGRATEDBKIMG) ) );
//     m_timer1.create( *this, 100 );

//     // ��������ͼ
//     m_memCanvas.create( m_rcClient.Width(), m_rcClient.Height() );
//     m_gCanvas.attachNew( new winplus::Graphics(m_memCanvas) );
//     m_gCanvas->SetSmoothingMode(SmoothingModeAntiAlias);
//     m_gCanvas->SetTextRenderingHint(TextRenderingHintAntiAlias);

//     // ��ͼ
//     MakeDraw();
// }

// // �����߼�
// void AccountIntegratedWnd::MakeDraw()
// {
//     // ������
//     m_gCanvas->DrawImage( m_loadedBgImg.get(), 0, 0, 0, 0, m_loadedBgImg->GetWidth(), m_loadedBgImg->GetHeight(), UnitPixel );

//     // �������Ч
//     //int x, y;
//     //x = m_ptCurMouse.x - m_radiusMouseCircle / 2;
//     //y = m_ptCurMouse.y - m_radiusMouseCircle / 2;
//     //m_gCanvas->FillEllipse( &m_brushHalfWhite, x, y, m_radiusMouseCircle, m_radiusMouseCircle );
//     //m_gCanvas->DrawEllipse( &m_penHalfWhite, x, y, m_radiusMouseCircle, m_radiusMouseCircle );

//     SolidBrush brushHalfBlack( Color( 32, 0, 0, 0 ) );
//     Gdiplus::Font font( L"΢���ź�", 10, 0, UnitPoint );
//     StringFormat fmt( StringFormat::GenericTypographic() );
//     fmt.SetAlignment(StringAlignmentNear);
//     fmt.SetLineAlignment(StringAlignmentCenter);
//     fmt.SetTrimming(StringTrimmingEllipsisCharacter);

//     // ��������
//     m_gCanvas->DrawShadowString( _T("��������˻�"), m_primaryFont, &m_brushWhite, &m_brushBlack, RectF( 0, 4, m_rcClient.Width(), 40 ), m_sfHVCenter, &m_captionRect );

//     // �����ݱ���
//     m_gCanvas->FillRoundRectangle( &m_brushHalfWhite, m_contentRect, 8 );
//     //DrawShadowFrame(m_contentRect);

//     // ���˻���Ϣ
//     int nAcountsCount = (int)m_accounts.GetCount();
//     int i = 0;
//     // �߾�
//     constexpr int padding = 4;
//     // �и�
//     constexpr int lineHeight = 48;
//     Pen *penWhite = m_penHalfWhite.Clone();
//     penWhite->SetDashStyle(DashStyleDot);
//     for ( int height = lineHeight; height < m_contentRect.Height && i < nAcountsCount; height += lineHeight, i++ )
//     {
//         AccountContext & accountCtx = m_accounts[i];

//         m_gCanvas->DrawLine( penWhite, accountCtx.accountRect.GetLeft(), accountCtx.accountRect.GetBottom(), accountCtx.accountRect.GetRight(), accountCtx.accountRect.GetBottom() );
//         m_gCanvas->FillRectangle( &brushHalfBlack, accountCtx.accountRect );
//         m_gCanvas->DrawShadowString( CStringToString(accountCtx.account.m_myName), font, &m_brushWhite, &brushHalfBlack, accountCtx.accountField0Rect, fmt, NULL );
//         if ( accountCtx.isPwdShown )
//         {
//             m_gCanvas->DrawShadowString( CStringToString(accountCtx.account.m_accountPwd), font, &SolidBrush( Color( 0, 255, 0 ) ), &brushHalfBlack, accountCtx.accountField1Rect, fmt, NULL );
//         }
//         else
//         {
//             m_gCanvas->DrawShadowString( CStringToString(accountCtx.account.m_accountName), font, &SolidBrush( Color( 255, 255, 0 ) ), &brushHalfBlack, accountCtx.accountField1Rect, fmt, NULL );
//         }

//         //if ( accountCtx.isSelected )
//         //{
//         //    DrawShadowFrame( accountCtx.accountRect, &Pen( Color( 200, 0, 127, 255 ), 2 ), NULL, 2 );
//         //}
//     }
//     delete penWhite;
// }

// BOOL AccountIntegratedWnd::PreCreateWindow(CREATESTRUCT& cs) 
// {
//     if ( !CWnd::PreCreateWindow(cs) ) return FALSE;
//     //cs.style &= ~( WS_THICKFRAME | WS_MAXIMIZEBOX | WS_OVERLAPPED | WS_BORDER );
//     //cs.style = WS_POPUP | WS_CAPTION | WS_SYSMENU;
//     cs.dwExStyle &= ~WS_EX_CLIENTEDGE;
//     cs.dwExStyle |= WS_EX_TOOLWINDOW;
//     //cs.lpszClass = AfxRegisterWndClass( CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS/* | CS_SAVEBITS*/ );
    
//     return TRUE;
// }

// BEGIN_MESSAGE_MAP(AccountIntegratedWnd, SpwWindow)
//     //{{AFX_MSG_MAP(AccountIntegratedWnd)
//     ON_WM_CREATE()
//     //ON_WM_NCHITTEST()
//     ON_WM_ERASEBKGND()
//     ON_WM_PAINT()
//     ON_WM_TIMER()
//     ON_WM_MOUSEMOVE()
//     ON_WM_LBUTTONDOWN()
//     ON_WM_LBUTTONUP()
//     ON_WM_RBUTTONDOWN()
//     ON_WM_RBUTTONUP()
//     //}}AFX_MSG_MAP
// END_MESSAGE_MAP()

// /////////////////////////////////////////////////////////////////////////////
// // AccountIntegratedWnd message handlers

// int AccountIntegratedWnd::OnCreate( LPCREATESTRUCT lpCreateStruct )
// {
//     if ( CWnd::OnCreate(lpCreateStruct) == -1 )
//         return -1;

//     // ����͸����
//     //SetLayeredWindowAttributes( 0, 255, LWA_ALPHA );

//     this->RefreshAllCreate();

//     return 0;
// }

// void AccountIntegratedWnd::OnPaint()
// {
//     CPaintDC dc(this);
//     Gdiplus::Graphics g(dc);
//     m_memCanvas.outputEntire( g, 0, 0 );
//     //m_memCanvas.copyEntireTo( dc, 0, 0 );
// }

// LRESULT AccountIntegratedWnd::OnNcHitTest( CPoint point )
// {
//     LRESULT ret = CWnd::OnNcHitTest(point);
//     //std::cout << "AccountIntegratedWnd::OnNcHitTest( CPoint point )" << ret << "\n";

//     // α������������ʹ�ÿ��Ե������λ���ƶ�����
//     if ( HTCLIENT == ret && GetAsyncKeyState(MK_LBUTTON) < 0 )
//     {
//         return HTCAPTION;
//     }
//     return ret;
// }

// BOOL AccountIntegratedWnd::OnEraseBkgnd( CDC * pDC )
// {
//     return TRUE; // CFrameWnd::OnEraseBkgnd(pDC);
// }

// void AccountIntegratedWnd::OnTimer( UINT_PTR nIDEvent )
// {
//     if ( nIDEvent == m_timer1.getId() )
//     {
//         static int incDelta = 1;
//         m_radiusMouseCircle += ++incDelta;
//         if ( m_radiusMouseCircle > 60 )
//         {
//             m_radiusMouseCircle = 10;
//             incDelta = 1;
//         }

//         MakeDraw();
//         Invalidate(FALSE);
//     }
// }

// void AccountIntegratedWnd::OnMouseMove( UINT nFlags, CPoint point )
// {
//     //std::cout << "AccountIntegratedWnd::OnMouseMove()\n";
//     m_ptCurMouse = point;

//     //CWnd::OnMouseMove(nFlags, point);
// }

// void AccountIntegratedWnd::OnLButtonDown( UINT nFlags, CPoint point )
// {
//     std::cout << "AccountIntegratedWnd::OnLButtonDown()\n";
//     //PlaySound( MAKEINTRESOURCE(IDR_WAVE_SELECTED), AfxGetApp()->m_hInstance, SND_RESOURCE | SND_ASYNC );
//     //CWnd::OnLButtonDown( nFlags, point );
// }

// void AccountIntegratedWnd::OnLButtonUp( UINT nFlags, CPoint point )
// {
//     std::cout << "AccountIntegratedWnd::OnLButtonUp(" << point.x << ", " << point.y << ")\n";

//     if ( m_captionRect.Contains( point.x, point.y ) )
//     {
//         //PlaySound( MAKEINTRESOURCE(IDR_WAVE_SELECTED), AfxGetApp()->m_hInstance, SND_RESOURCE | SND_ASYNC );
//         winplus::Mixed accountFields = winplus::$c{
//             { "cate", m_cate.m_id },
//             { "user", g_theApp.m_loginedUser.m_id }
//         };
//         ((MainFrame *)AfxGetMainWnd())->DoAddAccount( GetParent(), accountFields );

//         return;
//     }

//     for ( int i = 0; i < m_accounts.GetCount(); i++ )
//     {
//         if ( m_accounts[i].accountRect.Contains( point.x, point.y ) )
//         {
//             m_accounts[i].isPwdShown = !m_accounts[i].isPwdShown;
//             break;
//         }
//     }

//     MakeDraw();
//     Invalidate(FALSE);

//     //CWnd::OnLButtonUp( nFlags, point );
// }

// void AccountIntegratedWnd::OnRButtonDown( UINT nFlags, CPoint point )
// {
//     // TODO: �ڴ�������Ϣ������������/�����Ĭ��ֵ

//     //CWnd::OnRButtonDown( nFlags, point );
// }

// void AccountIntegratedWnd::OnRButtonUp( UINT nFlags, CPoint point )
// {
//     for ( int i = 0; i < m_accounts.GetCount(); i++ )
//     {
//         if ( m_accounts[i].accountRect.Contains( point.x, point.y ) )
//         {
//             CString data;
//             if ( m_accounts[i].isPwdShown )
//             {
//                 data = m_accounts[i].account.m_accountPwd;
//                 PlaySound( MAKEINTRESOURCE(IDR_WAVE_SE02), AfxGetApp()->m_hInstance, SND_RESOURCE | SND_ASYNC );
//             }
//             else
//             {
//                 data = m_accounts[i].account.m_accountName;
//                 PlaySound( MAKEINTRESOURCE(IDR_WAVE_SE01), AfxGetApp()->m_hInstance, SND_RESOURCE | SND_ASYNC );
//             }

//             // ���Ƶ�������
//             BOOL b = ::OpenClipboard( this->GetParent()->GetSafeHwnd() );

//             HANDLE hGlobalMem = GlobalAlloc( GMEM_MOVEABLE, data.GetLength() + 1 );
//             if ( !hGlobalMem )
//             {
//                 ::CloseClipboard();
//                 continue;
//             }
//             LPVOID lpData = GlobalLock(hGlobalMem);
//             if ( !lpData )
//             {
//                 GlobalFree(hGlobalMem);
//                 ::CloseClipboard();
//                 continue;
//             }
//             CopyMemory( lpData, data.GetString(), data.GetLength() );
//             GlobalUnlock(hGlobalMem);

//             EmptyClipboard();
//             SetClipboardData( CF_TEXT, hGlobalMem );

//             ::CloseClipboard();
//             break;
//         }
//     }

//     MakeDraw();
//     Invalidate(FALSE);
//     //CWnd::OnRButtonUp( nFlags, point );
// }
