import {
  Box,
  Button,
  Container,
  CssBaseline,
  Stack,
  Typography,
} from '@mui/material';
import { ThemeProvider } from '@mui/material/styles';
import { FormProvider, SubmitHandler, useForm } from 'react-hook-form';
import { array, object, TypeOf, z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import FileUpload from './FileUpload';
import { FC, useState } from 'react';
import { MediaObject } from '@/features/media/types';
const imageUploadSchema = object({
  imageCover: z.instanceof(File),
  images: array(z.instanceof(File)),
});

type IUploadImage = TypeOf<typeof imageUploadSchema>;

interface FileUploadFormProps {
  onSubmit: (...args: any) => any;
  setParentMediaList: (...args: any) => any;
}

const FileUploadForm: React.FC<FileUploadFormProps> = ({ onSubmit, setParentMediaList }) => {
  const methods = useForm<IUploadImage>({
    resolver: zodResolver(imageUploadSchema),
  });

  const getType = (file: File): 'image' | 'video' | 'pdf' => {
    if (file.type.startsWith('image')) return 'image';
    if (file.type.startsWith('video')) return 'video';
    if (file.type === 'application/pdf') return 'pdf';
    throw new Error('Unsupported file type');
  };

  const [mediaList, setMediaList] = useState<MediaObject[]>([]);

  const onSubmitHandler: SubmitHandler<IUploadImage> = async (values) => {
    const formData = new FormData();
    formData.append('imageCover', values.imageCover);

    if (values.images.length > 0) {
      values.images.forEach((el) => formData.append('images', el));
    }

    console.log(values);

    // Call the Upload API
    // uploadImage(formData);
    // const mediaList: MediaObject[] = values.images.map(image => ({
    //   url: URL.createObjectURL(image),
    //   file: image,
    //   type: getType(image),
    // }));

    console.log('I am the form submitter');

    // await onSubmit(mediaList);
  };

  return (
    <>
      <CssBaseline />
      <Container maxWidth={false}>
        <Box
          display='flex'
          sx={{
            justifyContent: 'center',
            alignItems: 'center',
            height: '30vh',
          }}
        >
          <Box display='flex' flexDirection='column' sx={{ width: '100%' }}>
            {/* Single Image Upload */}
            <FormProvider {...methods}>
              <Box
                component='form'
                noValidate
                autoComplete='off'
                onSubmit={methods.handleSubmit(onSubmitHandler)}
                // style={{ width: '300px' }}
                className='w-full'
              >
                {/* Multiple Image Upload */}
                <div className='flex justify-center items-center'>
                  <FileUpload
                    limit={3}
                    multiple 
                    name='images'
                    setParentMediaList={setParentMediaList}
                  />
                </div>
                {/* <Button
                  variant='contained'
                  type='submit'
                  fullWidth
                  sx={{ py: '0.8rem', my: 2 }}
                >
                  Submit Images
                </Button> */}
              </Box>
            </FormProvider>
          </Box>
        </Box>
      </Container>
    </>
  );
}

export default FileUploadForm;

