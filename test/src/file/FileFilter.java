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
    public static final String rex = "(?<=\")(\\s*select|\\s*join).*?(?=\")";
    public static final String sqlJoin = "join";
    public static final String sqlOrg = "(=\\s*)[a-z]*(.OrgId)";
    public static Pattern p = Pattern.compile(rex);
    public static Matcher m;
    //实现方法
    public static File[] Filter(File[] fileList){
        for (int i =0; i < fileList.length;i++){
            String line;
            //join语句条数
            int joinAllCount = 0;
            //OrgId条数
            int orgAllCount = 0;
            File file = fileList[i];
            long fileLength = file.length();
            byte[] fileContent = new byte[Integer.parseInt(String.valueOf(fileLength))];
            try{
                if (file.length() != 0){
                    stream = new FileInputStream(file);
                    stream.read(fileContent);
                    line = new String(fileContent,"UTF-8");
                    //消除所有换行和回车
                    line = line.replaceAll("\r|\n","");
                    m = p.matcher(line);
                    while (m.find()){
                        content.append(m.group());
                    }
                    //判断join语句中是否缺少OrgId
                    joinAllCount = CalculationCount(content.toString(),sqlJoin);
                    if (joinAllCount ==0)
                        continue;
                    orgAllCount = CalculationCount(content.toString(),sqlOrg);
                    if (joinAllCount != orgAllCount){
                        content.delete(0,content.length());
                        resultFiles.add(file);
                    }
                    else{
                        stream.close();
                        continue;
                    }
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        File files[] = new File[resultFiles.size()];
        resultFiles.toArray(files);
        return files;
    }
    //私有计数方法
    private static int CalculationCount(String fatherSql, String sonSql){
        int count = 0;
        Pattern pattern = Pattern.compile(sonSql);
        Matcher matcher = pattern.matcher(fatherSql);
        while (matcher.find()){
            count++;
        }
        return count;
    }
}
