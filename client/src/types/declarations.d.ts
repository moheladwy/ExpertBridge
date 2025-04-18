declare module 'react-ui-themes-superflows' {
  const Themes: any;
  export default Themes;
}

declare module 'react-upload-to-s3' {
  import * as React from 'react';

  interface UploadToS3Props {
    bucket: string;
    awsRegion: string;
    cognitoIdentityCredentials: {
      IdentityPoolId: string;
    };
    awsKey?: string;
    awsSecret?: string;
    awsMediaConvertEndPoint?: string;
    mediaConvertRole?: string;
    type?: 'image' | 'video' | 'all';
    theme?: any;
    showNewUpload?: boolean;
    onResult?: (result: {
      url: string;
      [key: string]: any;
    }) => void;
  }

  export const UploadToS3: React.FC<UploadToS3Props>;
}
