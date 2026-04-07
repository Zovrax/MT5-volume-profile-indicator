
// #include "Cassette.h"
// #include "CassetteApp.h"
// #include "AccountsView.h"

// #ifdef _DEBUG
// #define new DEBUG_NEW
// #undef THIS_FILE
// static char THIS_FILE[] = __FILE__;
// #endif

// /////////////////////////////////////////////////////////////////////////////
// // AccountsView

// AccountsView::AccountsView()
// {
// }

// AccountsView::~AccountsView()
// {
// }

// BOOL AccountsView::PreCreateWindow( CREATESTRUCT & cs ) 
// {
//     if ( !CListView::PreCreateWindow(cs) )
//         return FALSE;

//     cs.style |= LVS_REPORT | WS_TABSTOP | LVS_SINGLESEL | LVS_SHOWSELALWAYS;
//     return TRUE;
// }

// BEGIN_MESSAGE_MAP(AccountsView, CListView)
//     //{{AFX_MSG_MAP(AccountsView)
//     ON_WM_CREATE()
//     ON_WM_SIZE()
//     ON_NOTIFY_REFLECT(NM_RCLICK, OnListViewRClick)
//     ON_NOTIFY_REFLECT(NM_RETURN, OnListViewActivated)
//     ON_COMMAND(ID_LIST_STYLE_ICON, OnListStyleIcon)
//     ON_COMMAND(ID_LIST_STYLE_REPORT, OnListStyleReport)
//     ON_NOTIFY_REFLECT(NM_DBLCLK, OnListViewActivated)
//     //}}AFX_MSG_MAP
//     ON_UPDATE_COMMAND_UI_RANGE(ID_LIST_STYLE_REPORT, ID_LIST_STYLE_ICON, OnUpdateListStyle)
// END_MESSAGE_MAP()

// void AccountsView::UpdateList( int flag, long itemIndex )
// {
//     CListCtrl & lst = this->GetListCtrl();
//     int count = 0;

//     if ( flag & UPDATE_LOAD_DATA )
//     {
//         if ( itemIndex == -1 )
//         {
//             count = LoadAccounts( g_theApp.GetDatabase(), g_theApp.m_loginedUser.m_id, &m_accounts );
//         }
//         else
//         {
//             // ��myname��ȡ��¼
//             GetAccount(
//                 g_theApp.GetDatabase(),
//                 g_theApp.m_loginedUser.m_id,
//                 m_accounts[itemIndex].m_myName,
//                 &m_accounts[itemIndex]
//             );
//         }
//     }

//     if ( flag & UPDATE_LIST_ITEMS )
//     {
//         if ( itemIndex == -1 )
//         {
//             if ( !( flag & UPDATE_LOAD_DATA ) ) // ���û�������������Ҫ�������ȡ������Ϣ
//             {
//                 count = m_accounts.GetSize();
//             }
//             // Clear all items
//             lst.DeleteAllItems();
//             int i;
//             for ( i = 0; i < count; ++i )
//             {
//                 lst.InsertItem( i, m_accounts[i].m_myName, 0 );
//                 lst.SetItem( i, 1, LVIF_TEXT, m_accounts[i].m_accountName, 0, 0, 0, 0 );
//                 AccountType type;
//                 GetTypeByCateId( g_theApp.GetDatabase(), m_accounts[i].m_cateId, &type );
//                 lst.SetItem( i, 2, LVIF_TEXT, type.m_typeName, 0, 0, 0, 0 );
//                 lst.SetItem( i, 3, LVIF_TEXT, winplus::Format( _T("%d"), m_accounts[i].m_safeRank ).c_str(), 0, 0, 0, 0 );
//                 CTime dateTime(m_accounts[i].m_time);
//                 lst.SetItem( i, 4, LVIF_TEXT, dateTime.Format( _T("%Y-%m-%dT%H:%M:%S") ), 0, 0, 0, 0 );
//                 lst.SetItem( i, 5, LVIF_TEXT, m_accounts[i].m_comment, 0, 0, 0, 0 );
//             }
//         }
//         else
//         {
//             lst.SetItem( itemIndex, 0, LVIF_TEXT, m_accounts[itemIndex].m_myName, 0, 0, 0, 0 );
//             lst.SetItem( itemIndex, 1, LVIF_TEXT, m_accounts[itemIndex].m_accountName, 0, 0, 0, 0 );
//             AccountType type;
//             GetTypeByCateId( g_theApp.GetDatabase(), m_accounts[itemIndex].m_cateId, &type );
//             lst.SetItem( itemIndex, 2, LVIF_TEXT, type.m_typeName, 0, 0, 0, 0 );
//             lst.SetItem( itemIndex, 3, LVIF_TEXT, winplus::Format( _T("%d"), m_accounts[itemIndex].m_safeRank ).c_str(), 0, 0, 0, 0 );
//             CTime dateTime(m_accounts[itemIndex].m_time);
//             lst.SetItem( itemIndex, 4, LVIF_TEXT, dateTime.Format( _T("%Y-%m-%dT%H:%M:%S") ), 0, 0, 0, 0 );
//             lst.SetItem( itemIndex, 5, LVIF_TEXT, m_accounts[itemIndex].m_comment, 0, 0, 0, 0 );
//         }
//     }
// }

// /////////////////////////////////////////////////////////////////////////////
// // AccountsView message handlers
// int AccountsView::OnCreate( LPCREATESTRUCT lpCreateStruct ) 
// {
//     if ( CListView::OnCreate(lpCreateStruct) == -1 )
//         return -1;
//     CListCtrl & lst = this->GetListCtrl();

//     // ����ListView��ListCtrl����չ���
//     lst.SetExtendedStyle( LVS_EX_FULLROWSELECT );

//     // ��ʼ���б�ͷ
//     lst.InsertColumn( 0, _T("�˻�����"), LVCFMT_LEFT, 100 );
//     lst.InsertColumn( 1, _T("�˺�"), LVCFMT_LEFT, 100 );
//     lst.InsertColumn( 2, _T("���"), LVCFMT_LEFT, 48 );
//     lst.InsertColumn( 3, _T("��ȫȨֵ"), LVCFMT_LEFT, 48 );
//     lst.InsertColumn( 4, _T("¼��ʱ��"), LVCFMT_LEFT, 120 );
//     lst.InsertColumn( 5, _T("��ע"), LVCFMT_LEFT, 160 );

//     //lst.SetImageList();
//     m_iconList.Create( 48, 48, ILC_COLOR32 | ILC_MASK, 0, 1 );

//     m_iconList.Add( winplus::Icon(IDI_ICON1) );

//     lst.SetImageList( &m_iconList, LVSIL_NORMAL );

//     return 0;
// }

// void AccountsView::OnSize( UINT nType, int cx, int cy )
// {
//     CListView::OnSize( nType, cx, cy );
//     if ( nType != 0 || ( cx == 0 && cy == 0 ) )
//     {
//         return;
//     }
//     CListCtrl & lst = this->GetListCtrl();

//     BOOL b = ( GetWindowLong( GetSafeHwnd(), GWL_STYLE ) & LVS_REPORT ) != 0;
//     if ( b )
//     {
//         // ����Ӧ��С�ı�
//         CRect rc;
//         GetClientRect(&rc);
//         int widths[] = { 100, 100, 48, 48, 120, 160 };
//         double sumWidths = 0;
//         int i;
//         for ( i = 0; i < countof(widths); ++i )
//             sumWidths += widths[i];

//         for ( i = 0; i < countof(widths); ++i )
//         {
//             int newWidth = widths[i] / sumWidths * ( rc.Width() - GetSystemMetrics(SM_CXVSCROLL) );
//             newWidth = widths[i] > newWidth ? widths[i] : newWidth;
//             //�����û���SendMessage��SetColumnWidth()ʵ��,��ᵼ�¹���������,����PostMessage()��û����
//             //lst.SetColumnWidth( i, widths[i] > newWidth ? widths[i] : newWidth );
//             lst.PostMessage( LVM_SETCOLUMNWIDTH, i, MAKELPARAM( newWidth, 0 ) );
//         }
//     }
//     else
//     {
//         this->OnListStyleReport();
//         this->OnListStyleIcon();
//     }
// }

// void AccountsView::OnListViewRClick(NMHDR* pNMHDR, LRESULT* pResult) 
// {
//     CMenu menu;
//     menu.LoadMenu(IDM_OPERATE_ACCOUNTS);
//     POINT pt;
//     GetCursorPos(&pt);
//     menu.GetSubMenu(0)->TrackPopupMenu( 0, pt.x, pt.y, GetParentFrame() );
// }

// void AccountsView::OnListViewActivated( NMHDR * pNMHDR, LRESULT * pResult ) 
// {
//     CListCtrl & lst = GetListCtrl();
//     int index = lst.GetNextItem( -1, LVNI_ALL | LVNI_SELECTED );
//     if ( index == -1 )
//     {
//         return;
//     }
//     // ģ�����˵�����
//     GetParentFrame()->PostMessage( WM_COMMAND, MAKEWPARAM( ID_MODIFY_ACCOUNT, 0 ), 0 );
// }

// void AccountsView::OnListStyleIcon() 
// {
//     ModifyStyle( LVS_REPORT, LVS_ICON );
// }

// void AccountsView::OnListStyleReport() 
// {
//     ModifyStyle( LVS_ICON, LVS_REPORT );
// }

// void AccountsView::OnUpdateListStyle( CCmdUI * pCmdUI )
// {
//     switch ( pCmdUI->m_nID )
//     {
//     case ID_LIST_STYLE_REPORT:
//         {
//             BOOL b = ( GetWindowLong( GetSafeHwnd(), GWL_STYLE ) & LVS_REPORT ) != FALSE;
//             pCmdUI->SetRadio(b);
//         }
//         break;
//     case ID_LIST_STYLE_ICON:
//         {
//             BOOL b = ( GetWindowLong( GetSafeHwnd(), GWL_STYLE ) & LVS_REPORT ) != LVS_REPORT;
//             pCmdUI->SetRadio(b);
//         }
//         break;
//     }
// }
