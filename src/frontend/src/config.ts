export type ConfigProps = {
  Sidebar_drawer: boolean;
  Customizer_drawer: boolean;
  mini_sidebar: boolean;
  fontTheme: string;
  inputBg: boolean;
  defaultLocale: string;
};

const config: ConfigProps = {
  Sidebar_drawer: true,
  Customizer_drawer: false,
  mini_sidebar: false,
  fontTheme: 'Roboto',
  inputBg: false,
  defaultLocale: 'en'
};

export default config;
