import { useState } from 'react';

import { Button, Layout } from 'antd';
import { MenuUnfoldOutlined, MenuFoldOutlined } from '@ant-design/icons';
import { Content, Header } from 'antd/es/layout/layout';

import Sidebar from './components/Sidebar';
import ToggleThemeButton from './components/ToggleThemeButton';
import PDFViewer from './components/PDFViewer';

const { Sider } = Layout;

function App() {
  const [darkTheme, setDarkTheme] = useState(true);
  const [collapsed, setCollapsed] = useState(true);
  const [selectedKey, setSelectedKey] = useState('home'); // Default to 'home'

  const toggleTheme = () => {
    setDarkTheme(!darkTheme);
  };


  const handleMenuSelect = (key: string) => {
    setSelectedKey(key);
  };

  return (
    <>
      <Layout>
        <Sider
          collapsed={collapsed}
          collapsible
          trigger={null}
          theme={darkTheme ? 'dark' : 'light'}
          className='sidebar'
          width={250}
          style={{
            height: '100vh', 
            position: 'fixed',
            left: 0,
            top: 0,
            zIndex: 1000,
            overflow: 'hidden',
          }}
        >
          <Sidebar darkTheme={darkTheme} onSelect={handleMenuSelect} />
          <ToggleThemeButton darkTheme={darkTheme} toggleTheme={toggleTheme} />
        </Sider>

        <Layout style={{ marginLeft: collapsed ? 80 : 250 }}>
          <Header style={{ padding: 0, background: darkTheme? '' : 'white' }}>
          <Button
            type='text'
            className='toggle'
            onClick={() => setCollapsed(!collapsed)}
            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            style={{
              backgroundColor: 'white',
              border: '1px solid #d9d9d9',
              padding: '8px 16px',
              borderRadius: '4px'
            }}
          />
          </Header>
          <Content
            style={{
              padding: '24px',
              overflowY: 'auto', 
              height: '100vh', 
              marginTop: 64,
            }}
          >
            {selectedKey === 'home' && <div>Home Content</div>}
            {selectedKey === 'enrollment' && <div>Enrollment</div>}
            {selectedKey === 'all-olympiads' && (
              <PDFViewer />
            )}
            {selectedKey === 'documents' && <div>Documents</div>}
            {selectedKey === 'for-me' && <div>For Me</div>}
            {selectedKey === 'settings' && <div>Settings Content</div>}
            {selectedKey === 'my-profile' && <div>My Profile Content</div>}
          </Content>
        </Layout>
      </Layout>
    </>
  );
}

export default App;