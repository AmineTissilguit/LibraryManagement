import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';

const useDocumentTitle = () => {
  const location = useLocation();

  useEffect(() => {
    const getPageTitle = (pathname: string): string => {
      const appName = 'Library Management';
      
      switch (pathname) {
        case '/':
        case '/books':
          return `${appName} - Books`;
        case '/members':
          return `${appName} - Members`;
        case '/transactions':
          return `${appName} - Transactions`;
        default:
          return appName;
      }
    };

    document.title = getPageTitle(location.pathname);
  }, [location.pathname]);
};

export default useDocumentTitle;