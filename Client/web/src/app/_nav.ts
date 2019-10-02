export interface NavData {
  name?: string;
  url?: string;
  icon?: string;
  badge?: any;
  title?: boolean;
  children?: any;
  variant?: string;
  attributes?: object;
  divider?: boolean;
  class?: string;
}

export const navItems: NavData[] = [
  {
    name: 'Dashboard',
    url: '/dashboard',
    icon: 'icon-speedometer',
    badge: {
      variant: 'info',
      text: 'NEW'
    }
  },
  // {
  //   title: true,
  //   name: 'Company'
  // },
  {
    name: 'Company',
    children: [
      {
        name: 'Company',
        url: '/companies',
        icon: 'fa fa-building-o'
      },
      {
        name: 'Parts',
        url: '/parts',
        icon: 'icon-puzzle'
      },
      {
        name: 'Inventory',
        url: '/companies/inventory',
        icon: 'icon-puzzle'
      },
      {
        name: 'Invoice',
        url: '/companies/invoice',
        icon: 'icon-puzzle'
      }
    ]
  },
  {
    name: 'Purchase',
    children: [
      {
        name: 'Supplier',
        url: '/suppliers',
        icon: 'icon-puzzle'
      },
      {
        name: 'Supplier PO',
        url: '/suppliers/purchase-order/0/0',
        icon: 'icon-puzzle'
      },
      {
        name: 'Invoice',
        url: '/invoice',
        icon: 'icon-puzzle'
      }
    ]
  },
  {
    name: 'Orders',
    url: '/orders/detail/all/0'
  },
  {
    name: 'Sale',
    children: [
      {
        name: 'Customer',
        url: '/customers',
        icon: 'icon-puzzle'
      },
      {
        name: 'Shipments',
        url: '/companies/shipment-list',
        icon: 'icon-puzzle'
      },
      // {
      //   name: 'Create Shipment',
      //   url: '/companies/create-shipment',
      //   icon: 'icon-puzzle'
      // },
      {
        name: 'Barcode',
        url: '/barcode',
        icon: 'icon-puzzle'
      },
      {
        name: 'QR Code',
        url: '/barcode/qrcode',
        icon: 'icon-puzzle'
      }
    ]
  },
  {
    name: 'Report',
    children: [
      {
        name: 'Purchase',
        url: '/report/purchase',
        icon: 'icon-user'
      },
      {
        name: 'Sale',
        url: '/report/sale',
        icon: 'icon-user'
      },
      {
        name: 'Admin',
        url: '/report/admin',
        icon: 'icon-user'
      }
    ]
  },
  {
    name: 'Admin',
    children: [
      {
        name: 'User',
        url: '/admin',
        icon: 'icon-user'
      },
      {
        name: 'Privilages',
        url: '/admin',
        icon: 'icon-user'
      }
    ]
  },
  // {
  //   name: 'Products',
  //   url: '/products',
  //   icon: 'icon-puzzle'
  // },
  // {
  //   title: true,
  //   name: 'Reports'
  // },
  // {
  //   name: 'Reports',
  //   url: '/reports',
  //   icon: 'cui-graph'
  // },
  // {
  //   name: 'Colors',
  //   url: '/theme/colors',
  //   icon: 'icon-drop'
  // },
  // {
  //   name: 'Typography',
  //   url: '/theme/typography',
  //   icon: 'icon-pencil'
  // },
  // {
  //   title: true,
  //   name: 'Components'
  // },
  // {
  //   name: 'Base',
  //   url: '/base',
  //   icon: 'icon-puzzle',
  //   children: [
  //     {
  //       name: 'Cards',
  //       url: '/base/cards',
  //       icon: 'icon-puzzle',
  //       children: [
  //         {
  //           name: 'Sub Menu 1',
  //           url: '/base/carousels',
  //           icon: 'icon-puzzle'
  //         },{
  //           name: 'Sub Menu 2',
  //           url: '/base/carousels',
  //           icon: 'icon-puzzle'
  //         }
  //       ]
  //     },
  //     {
  //       name: 'Carousels',
  //       url: '/base/carousels',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Collapses',
  //       url: '/base/collapses',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Forms',
  //       url: '/base/forms',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Pagination',
  //       url: '/base/paginations',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Popovers',
  //       url: '/base/popovers',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Progress',
  //       url: '/base/progress',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Switches',
  //       url: '/base/switches',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Tables',
  //       url: '/base/tables',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Tabs',
  //       url: '/base/tabs',
  //       icon: 'icon-puzzle'
  //     },
  //     {
  //       name: 'Tooltips',
  //       url: '/base/tooltips',
  //       icon: 'icon-puzzle'
  //     }
  //   ]
  // },
  // {
  //   name: 'Buttons',
  //   url: '/buttons',
  //   icon: 'icon-cursor',
  //   children: [
  //     {
  //       name: 'Buttons',
  //       url: '/buttons/buttons',
  //       icon: 'icon-cursor'
  //     },
  //     {
  //       name: 'Dropdowns',
  //       url: '/buttons/dropdowns',
  //       icon: 'icon-cursor'
  //     },
  //     {
  //       name: 'Brand Buttons',
  //       url: '/buttons/brand-buttons',
  //       icon: 'icon-cursor'
  //     }
  //   ]
  // },
  // {
  //   name: 'Charts',
  //   url: '/charts',
  //   icon: 'icon-pie-chart'
  // },
  // {
  //   name: 'Icons',
  //   url: '/icons',
  //   icon: 'icon-star',
  //   children: [
  //     {
  //       name: 'CoreUI Icons',
  //       url: '/icons/coreui-icons',
  //       icon: 'icon-star',
  //       badge: {
  //         variant: 'success',
  //         text: 'NEW'
  //       }
  //     },
  //     {
  //       name: 'Flags',
  //       url: '/icons/flags',
  //       icon: 'icon-star'
  //     },
  //     {
  //       name: 'Font Awesome',
  //       url: '/icons/font-awesome',
  //       icon: 'icon-star',
  //       badge: {
  //         variant: 'secondary',
  //         text: '4.7'
  //       }
  //     },
  //     {
  //       name: 'Simple Line Icons',
  //       url: '/icons/simple-line-icons',
  //       icon: 'icon-star'
  //     }
  //   ]
  // },
  // {
  //   name: 'Notifications',
  //   url: '/notifications',
  //   icon: 'icon-bell',
  //   children: [
  //     {
  //       name: 'Alerts',
  //       url: '/notifications/alerts',
  //       icon: 'icon-bell'
  //     },
  //     {
  //       name: 'Badges',
  //       url: '/notifications/badges',
  //       icon: 'icon-bell'
  //     },
  //     {
  //       name: 'Modals',
  //       url: '/notifications/modals',
  //       icon: 'icon-bell'
  //     }
  //   ]
  // },
  // {
  //   name: 'Widgets',
  //   url: '/widgets',
  //   icon: 'icon-calculator',
  //   badge: {
  //     variant: 'info',
  //     text: 'NEW'
  //   }
  // },
  {
    divider: true
  },
  // {
  //   title: true,
  //   name: 'Extras',
  // },
  // {
  //   name: 'Pages',
  //   url: '/pages',
  //   icon: 'icon-star',
  //   children: [
  //     {
  //       name: 'Login',
  //       url: '/login',
  //       icon: 'icon-star'
  //     },
  //     {
  //       name: 'Register',
  //       url: '/register',
  //       icon: 'icon-star'
  //     },
  //     {
  //       name: 'Error 404',
  //       url: '/404',
  //       icon: 'icon-star'
  //     },
  //     {
  //       name: 'Error 500',
  //       url: '/500',
  //       icon: 'icon-star'
  //     }
  //   ]
  // },
  // {
  //   name: 'Disabled',
  //   url: '/dashboard',
  //   icon: 'icon-ban',
  //   badge: {
  //     variant: 'secondary',
  //     text: 'NEW'
  //   },
  //   attributes: { disabled: true },
  // }
];
