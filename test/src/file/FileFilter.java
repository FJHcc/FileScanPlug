package file;

import java.io.*;
import java.util.ArrayList;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class FileFilter {
    //用来存放过滤后的文件
    public static ArrayList<File> resultFiles = new ArrayList<>();
    public static FileInputStream stream = null;
    public static StringBuilder content = new StringBuilder();
    public static final String rex = "^join";
    public static Pattern p = Pattern.compile(rex);
    public static Matcher m;
    //实现方法
    public static File[] Filter(File[] fileList){
        for (int i =0; i < fileList.length;i++){
            String line;
            File file = fileList[i];
            long fileLength = file.length();
            byte[] fileContent = new byte[Integer.parseInt(String.valueOf(fileLength))];
            try{
                if (file.length() != 0){
                    stream = new FileInputStream(file);
                    stream.read(fileContent);
                    line = new String(fileContent,"UTF-8");
                    m = p.matcher(line);
                    while (m.find()){
                        content.append(m.group());
                    }
                    System.out.println(content);

                   /* InputStreamReader streamReader = new InputStreamReader(stream);
                    BufferedReader bufferedReader = new BufferedReader(streamReader);*/
                   /* while ((line = bufferedReader.readLine())!= null){
                        if (line.contains("join")){
                            content.append(line);
                            //join语句中没有OrgId 的话就过滤出
                            if (!content.toString().contains(".OrgId")){
                                //清空StringBuilder
                                content.delete(0,content.length());
                                resultFiles.add(file);
                            }
                        }
                        else {
                            continue;
                        }
                    }*/
                   //bufferedReader.close();
                    stream.close();
                }
            } catch (Exception e) {
                e.printStackTrace();
            }

        }
        File files[] = new File[resultFiles.size()];
        resultFiles.toArray(files);
        return files;
    }
}
